using System;
using System.Collections.Generic;
using System.Linq;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Tools.ReportGenerator;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

namespace Gabo;

[ShutdownDotNetAfterServerBuild]
sealed class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode

    public static int Main() => Execute<Build>(x => x.Pack);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Parameter("Whether we create the NuGet package - Default is false")]
    readonly bool Package;

    [Solution] readonly Solution Solution;
    [GitVersion] readonly GitVersion GitVersion;

    static AbsolutePath SourceDirectory => RootDirectory / "src";
    static AbsolutePath TestsDirectory => RootDirectory / "tests";
    static AbsolutePath ArtifactsDirectory => RootDirectory / "artifacts";
    static AbsolutePath TestResultsDirectory => ArtifactsDirectory / "test-results";
    static AbsolutePath CodeCoverageDirectory => ArtifactsDirectory / "coverage-report";
    static AbsolutePath PackagesDirectory => ArtifactsDirectory / "packages";

#pragma warning disable CA1822 // Can't make this static as it breaks NUKE
    Target Clean => _ => _
#pragma warning restore CA1822
        .Executes(() =>
        {
            SourceDirectory.GlobDirectories("**/bin", "**/obj").ForEach(p => p.DeleteDirectory());
            TestsDirectory.GlobDirectories("**/bin", "**/obj").ForEach(p => p.DeleteDirectory());
            ArtifactsDirectory.CreateOrCleanDirectory();
        });

    Target Restore => _ => _
        .DependsOn(Clean)
        .Executes(() =>
        {
            DotNetRestore(s => s.SetProjectFile(Solution));
        });

    Target SetGitHubVersion => _ => _
        .DependsOn(Restore)
        .OnlyWhenStatic(() => !IsLocalBuild)
        .Executes(() =>
        {
            var gitHubEnvironmentFile = Environment.GetEnvironmentVariable("GITHUB_ENV");
            var packageVersionEnvironmentVariable = $"PACKAGE_VERSION={GitVersion.NuGetVersionV2}";
            System.IO.File.WriteAllText(gitHubEnvironmentFile, packageVersionEnvironmentVariable);
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .DependsOn(SetGitHubVersion)
        .Executes(() =>
        {
            Serilog.Log.Information("AssemblySemVer: {AssemblyVersion}", GitVersion.AssemblySemVer);
            Serilog.Log.Information("NuGetVersion: {AssemblyVersion}", GitVersion.NuGetVersion);

            DotNetBuild(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .SetAssemblyVersion(GitVersion.AssemblySemVer)
                .SetFileVersion(GitVersion.NuGetVersion)
                .SetInformationalVersion(GitVersion.NuGetVersion)
                .EnableNoRestore()
                .EnableNoIncremental());
        });

    Target VerifyFormat => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            DotNet($"format --verify-no-changes");
        });

    Target Test => _ => _
        .DependsOn(VerifyFormat)
        .Executes(() =>
        {
            var testProjects =
                from testProject in Solution.AllProjects
                where testProject.Name.EndsWith("Tests", StringComparison.Ordinal)
                from framework in testProject.GetTargetFrameworks()
                select new { TestProject = testProject, Framework = framework };

            DotNetTest(s => s
                .SetConfiguration(Configuration)
                .SetDataCollector("XPlat Code Coverage")
                .EnableNoBuild()
                .CombineWith(testProjects, (ss, p) =>
                {
                    var testResultsName = $"{p.TestProject.Path.NameWithoutExtension}-{p.Framework}";
                    var testResultsDirectory = TestResultsDirectory / testResultsName;

                    var loggers = new List<string>
                    {
                        $"trx;LogFileName={testResultsName}.trx",
                        $"html;LogFileName={testResultsName}.html"
                    };

                    if (IsServerBuild)
                    {
                        loggers.Add($"GitHubActions;annotations.titleFormat=$test ({p.Framework})");
                    }

                    return ss
                        .SetProjectFile(p.TestProject)
                        .SetFramework(p.Framework)
                        .SetResultsDirectory(testResultsDirectory)
                        .SetLoggers(loggers);
                }), completeOnFailure: true);
        });

    Target Coverage => _ => _
        .DependsOn(Test)
        .Executes(() =>
        {
            ReportGeneratorTasks.ReportGenerator(s => s
                .SetFramework("net8.0")
                .SetReports($"{TestResultsDirectory}/**/coverage.cobertura.xml")
                .SetTargetDirectory(CodeCoverageDirectory)
                .SetReportTypes(ReportTypes.Html));
        });

    Target Pack => _ => _
        .DependsOn(Coverage)
        .OnlyWhenStatic(() => Package)
        .Executes(() =>
        {
            DotNetPack(s => s
                .SetProject(SourceDirectory / "xUnitToJUnit")
                .SetConfiguration(Configuration)
                .EnableNoBuild()
                .EnableIncludeSymbols()
                .SetOutputDirectory(PackagesDirectory)
                .SetVersion(GitVersion.NuGetVersion));
        });
}
