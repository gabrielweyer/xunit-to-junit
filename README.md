# xUnit.net v2 XML Format to JUnit Format #

| Package | Release | Pre-release |
| --- | --- | --- |
| `dotnet-xunit-to-junit` | [![NuGet][nuget-tool-badge]][nuget-tool-command] | [![MyGet][myget-tool-badge]][myget-tool-command] |

| CI | Status | Platform(s) | Framework(s) | Test Framework(s) |
| --- | --- | --- | --- | --- |
| [AppVeyor][app-veyor] | [![Build Status][app-veyor-shield]][app-veyor] | `Windows` | `nestandard2.0` | `netcoreapp2.1.0` |

[CircleCI][circle-ci] can only parse test results in the [JUnit format][junit-format]. This `Extensible Stylesheet Language Transformations` can transform a `xUnit.net v2 XML` test results file into a `JUnit` test results file.

**Note**: this only handles the easiest use case for the moment, as soon as I encounter issues in real life usage I'll add extra testing scenarios.

## Consume the transform ##

### Consume `JUnit.xslt` through the `dotnet-xunit-to-junit` `NuGet` package ###

`dotnet-xunit-to-junit` is a `.NET Core` [global tool][dotnet-global-tools]:

```posh
dotnet tool install -g dotnet-xunit-to-junit
dotnet xunit-to-junit "path-to-xunit-test-results.xml" "desired-path-to-junit-test-results.xml"
```

### Consume `JUnit.xslt` directly from C# ###

**Note**: For `.NET Core`, this requires `nestandard2.0` and above.

```csharp
// Required using statement
using System.Xml.Xsl;

// Change the value of these three variables
const string inputFilePath = "C:/tmp/xunit.xml";
const string outputFilePath = "C:/tmp/junit.xml";
const string xsltFilePath = "C:/tmp/JUnit.xslt";

var xlsTransform = new XslCompiledTransform();
xlsTransform.Load(xsltFilePath);

writerSettings = xlsTransform.OutputSettings.Clone();
// Save without BOM, CircleCI can't read test results files starting with a BOM
writerSettings.Encoding = new UTF8Encoding(false);

using (var stream = new FileStream(outputFilePath, FileMode.Create, FileAccess.Write))
using (var results = XmlWriter.Create(stream, writerSettings))
{
    xlsTransform.Transform(inputFilePath, results);
}
```

[circle-ci]: https://circleci.com/
[junit-format]: http://llg.cubic.org/docs/junit/
[nuget-tool-badge]: https://img.shields.io/nuget/v/dotnet-xunit-to-junit.svg?label=NuGet
[nuget-tool-command]: https://www.nuget.org/packages/dotnet-xunit-to-junit
[myget-tool-badge]: https://img.shields.io/myget/gabrielweyer-pre-release/v/dotnet-xunit-to-junit.svg?label=MyGet
[myget-tool-command]: https://www.myget.org/feed/gabrielweyer-pre-release/package/nuget/dotnet-xunit-to-junit
[app-veyor]: https://ci.appveyor.com/project/GabrielWeyer/xunit-to-junit
[app-veyor-shield]: https://ci.appveyor.com/api/projects/status/github/gabrielweyer/xunit-to-junit?branch=master&svg=true
[dotnet-global-tools]: https://docs.microsoft.com/en-us/dotnet/core/tools/global-tools
