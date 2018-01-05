# xUnit.net v2 XML Format to JUnit Format #

[CircleCI][circle-ci] can only parse test results in the [JUnit format][junit-format]. This `Extensible Stylesheet Language Transformations` can transform a `xUnit.net v2 XML` test results file into a `JUnit` test results file.

**Note**: this only handles the easiest use case for the moment, as soon as I encounter issues in real life usage I'll add extra testing scenarios.

## Consume the transform ##

### Consume `JUnit.xslt` through the `xUnitToJUnit.CommandLine` `NuGet` package ###

```posh
dotnet ./xunit-to-junit.dll "path-to-xunit-test-results.xml" "desired-path-to-junit-test-results.xml"
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
