# xUnit.net v2 XML Format to JUnit Format #

:rotating_light: If you're using this tool to process test results file(s) after having run `dotnet test`, I recommend using the [JUnit Test Logger][junit-logger] instead. The output is better (at least in CircleCI). If you can't install an additional NuGet package in your test project(s), can't modify your build or are processing test results file(s) that have previously been generated, please read on.

| Package                 | Release                                          |
| ----------------------- | ------------------------------------------------ |
| `dotnet-xunit-to-junit` | [![NuGet][nuget-tool-badge]][nuget-tool-command] |

| CI                       | Status                                                   | Platform(s) | Framework(s) | Test Framework(s) |
| ------------------------ | -------------------------------------------------------- | ----------- | ------------ | ----------------- |
| [GitHub][github-actions] | [![Build Status][github-actions-shield]][github-actions] | `Ubuntu`    | `net8.0`     | `net8.0`          |

[CircleCI][circle-ci] can only parse test results in the [JUnit format][junit-format]. This `Extensible Stylesheet Language Transformations` can transform a `xUnit.net v2 XML` test results file into a `JUnit` test results file.

**Note**: this only handles the easiest use case for the moment, as soon as I encounter issues in real life usage I'll add extra testing scenarios.

## Consume the transform ##

### Consume `JUnit.xslt` through the `dotnet-xunit-to-junit` `NuGet` package ###

`dotnet-xunit-to-junit` is a `.NET` [global tool][dotnet-global-tools]:

```powershell
dotnet tool install -g dotnet-xunit-to-junit
dotnet xunit-to-junit "path-to-xunit-test-results.xml" "desired-path-to-junit-test-results.xml"
```

### Consume `JUnit.xslt` directly from C# ###

```csharp
// Required using statement
using System.Xml.Xsl;

// Change the value of these three variables
const string inputFilePath = "C:/tmp/xunit.xml";
const string outputFilePath = "C:/tmp/junit.xml";
const string xsltFilePath = "C:/tmp/JUnit.xslt";

var xlsTransform = new XslCompiledTransform();
xlsTransform.Load(xsltFilePath);

var writerSettings = xlsTransform.OutputSettings.Clone();
// Save without BOM, CircleCI can't read test results files starting with a BOM
writerSettings.Encoding = new UTF8Encoding(false);

using (var stream = new FileStream(outputFilePath, FileMode.Create, FileAccess.Write))
using (var results = XmlWriter.Create(stream, writerSettings))
{
    xlsTransform.Transform(inputFilePath, results);
}
```

## Building locally ##

Run this command to build on Windows:

```powershell
.\build.ps1
```

Run this command to build on Linux / macOS:

```shell
./build.sh
```

If you want to pack the `.NET Global Tool`, you can run `.\build.ps1 --package`.

[github-actions]: https://github.com/gabrielweyer/xunit-to-junit/actions/workflows/build.yml
[github-actions-shield]: https://github.com/gabrielweyer/xunit-to-junit/actions/workflows/build.yml/badge.svg
[circle-ci]: https://circleci.com/
[junit-format]: http://llg.cubic.org/docs/junit/
[nuget-tool-badge]: https://img.shields.io/nuget/v/dotnet-xunit-to-junit.svg?label=NuGet
[nuget-tool-command]: https://www.nuget.org/packages/dotnet-xunit-to-junit
[dotnet-global-tools]: https://docs.microsoft.com/en-us/dotnet/core/tools/global-tools
[junit-logger]: https://github.com/spekt/junit.testlogger
