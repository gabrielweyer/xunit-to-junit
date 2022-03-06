#tool dotnet:?package=GitVersion.Tool&version=5.8.2
#addin nuget:?package=Cake.Incubator&version=7.0.0

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

var assemblyVersion = "1.0.0";
var packageVersion = "1.0.0";

var artifactsDir = MakeAbsolute(Directory("artifacts"));
var testsResultsDir = artifactsDir.Combine(Directory("tests-results"));
var packagesDir = artifactsDir.Combine(Directory("packages"));
var testResultsDir = artifactsDir.Combine(Directory("test-results"));

var solutionPath = "./xUnitToJUnit.sln";

var testProjects = new List<TestProject>();

Task("Clean")
    .Does(() =>
    {
        CleanDirectory(testsResultsDir);

        var settings = new DotNetCoreCleanSettings
        {
            Configuration = configuration
        };

        DotNetCoreClean(solutionPath, settings);
    });

Task("Restore")
    .IsDependentOn("Clean")
    .Does(() =>
    {
        DotNetCoreRestore();
    });

Task("SemVer")
    .IsDependentOn("Restore")
    .Does(() =>
    {
        var gitVersion = GitVersion();

        assemblyVersion = gitVersion.AssemblySemVer;
        packageVersion = gitVersion.NuGetVersion;

        Information($"AssemblySemVer: {assemblyVersion}");
        Information($"NuGetVersion: {packageVersion}");
    });

Task("SetGitHubVersion")
    .IsDependentOn("Semver")
    .WithCriteria(() => GitHubActions.IsRunningOnGitHubActions)
    .Does(() =>
    {
        var gitHubEnvironmentFile = Environment.GetEnvironmentVariable("GITHUB_ENV");
        var packageVersionEnvironmentVariable = $"PACKAGE_VERSION={packageVersion}";
        System.IO.File.WriteAllText(gitHubEnvironmentFile, packageVersionEnvironmentVariable);
    });

Task("Build")
    .IsDependentOn("SetGitHubVersion")
    .Does(() =>
    {
        var settings = new DotNetCoreBuildSettings
        {
            Configuration = configuration,
            NoIncremental = true,
            NoRestore = true,
            MSBuildSettings = new DotNetCoreMSBuildSettings()
                .SetVersion(assemblyVersion)
                .WithProperty("FileVersion", packageVersion)
                .WithProperty("InformationalVersion", packageVersion)
                .WithProperty("nowarn", "7035")
        };

        DotNetCoreBuild(solutionPath, settings);
    });

Task("ListTestProjectsAndFrameworkVersions")
    .IsDependentOn("Build")
    .DoesForEach(GetFiles("./tests/*/*Tests.csproj"), (testProject) =>
    {
        var parsedProject = ParseProject(testProject.FullPath, configuration: configuration);

        parsedProject.TargetFrameworkVersions.ToList().ForEach(frameworkVersion =>
        {
            var projectToTest = new TestProject
            {
                FullPath = testProject.FullPath,
                AssemblyName = parsedProject.AssemblyName,
                FrameworkVersion = frameworkVersion
            };
            testProjects.Add(projectToTest);
        });
    });

Task("Test")
    .IsDependentOn("ListTestProjectsAndFrameworkVersions")
    .DoesForEach(() => testProjects, (testProject) =>
    {
        var settings = new DotNetTestSettings
        {
            Configuration = configuration,
            NoBuild = true,
            Framework = testProject.FrameworkVersion
        };

        if (GitHubActions.IsRunningOnGitHubActions)
        {
            var trxTestResultsFile = testResultsDir
                .Combine($"{testProject.AssemblyName}.{testProject.FrameworkVersion}.trx");
            settings.Loggers.Add($"\"trx;LogFileName={trxTestResultsFile}\"");
        }

        DotNetTest(testProject.FullPath, settings);
    })
    .DeferOnError();

Task("Pack")
    .IsDependentOn("Test")
    .WithCriteria(() => HasArgument("pack"))
    .Does(() =>
    {
        var settings = new DotNetCorePackSettings
        {
            Configuration = configuration,
            NoBuild = true,
            NoRestore = true,
            IncludeSymbols = true,
            OutputDirectory = packagesDir,
            MSBuildSettings = new DotNetCoreMSBuildSettings()
                .WithProperty("PackageVersion", packageVersion)
        };

        GetFiles("./src/*/*.csproj")
            .ToList()
            .ForEach(f => DotNetCorePack(f.FullPath, settings));
    });

Task("Default")
    .IsDependentOn("Pack");

RunTarget(target);

class TestProject
{
    public string FullPath { get; set; }
    public string AssemblyName { get; set; }
    public string FrameworkVersion { get; set; }
}
