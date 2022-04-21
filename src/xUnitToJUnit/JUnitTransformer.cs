using System.Text;
using System.Xml;
using System.Xml.Xsl;

namespace Gabo.DotNet.xUnitToJUnit;

/// <summary>
/// Transforms a `xUnit.net v2 XML` test results file into a `JUnit` test results file.
/// </summary>
public static class JUnitTransformer
{
    private static readonly XmlWriterSettings _writerSettings;
    private static readonly XslCompiledTransform _xlsTransform;

    static JUnitTransformer()
    {
        _xlsTransform = new XslCompiledTransform();
        var xsltPath = $"{AppContext.BaseDirectory}/JUnit.xslt";
        _xlsTransform.Load(xsltPath);

        if (_xlsTransform.OutputSettings == null)
        {
            throw new InvalidOperationException($"The XSLT does not contain a xsl:output element ('{xsltPath}').");
        }

        _writerSettings = _xlsTransform.OutputSettings.Clone();
        _writerSettings.Encoding = new UTF8Encoding(false);
    }

    /// <summary>
    /// Transforms a `xUnit.net v2 XML` test results file into a `JUnit` test results file.
    /// </summary>
    /// <param name="xUnitTestResultsFilePath">The `xUnit.net v2 XML` test results file path.</param>
    /// <param name="jUnitTestResultsFilePath">The `JUnit` test results file path, if the containing
    /// directory does not exist it will be created.</param>
    public static void Transform(string xUnitTestResultsFilePath, string jUnitTestResultsFilePath)
    {
        var jUnitTestResultsDirectory = Path.GetDirectoryName(jUnitTestResultsFilePath);

        if (!string.IsNullOrEmpty(jUnitTestResultsDirectory) && !Directory.Exists(jUnitTestResultsDirectory))
        {
            Directory.CreateDirectory(jUnitTestResultsDirectory);
        }

        using (var stream = new FileStream(jUnitTestResultsFilePath, FileMode.Create, FileAccess.Write))
        {
            Transform(xUnitTestResultsFilePath, stream);
        }
    }

    /// <summary>
    /// Transforms a `xUnit.net v2 XML` test results file into the `JUnit` format and write the result
    /// to a Stream.
    /// </summary>
    /// <param name="xUnitTestResultsFilePath">The `xUnit.net v2 XML` test results file path.</param>
    /// <param name="stream">The output Stream.</param>
    public static void Transform(string xUnitTestResultsFilePath, Stream stream)
    {
        using (var results = XmlWriter.Create(stream, _writerSettings))
        {
            _xlsTransform.Transform(xUnitTestResultsFilePath, results);
        }
    }
}
