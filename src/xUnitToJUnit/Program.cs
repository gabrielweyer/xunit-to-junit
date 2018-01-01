using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Xsl;

namespace xUnitToJUnit
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Two arguments should be provided:");
                Console.WriteLine(
                    "dotnet ./xunit-to-junit.dll \"path-to-xunit-test-results.xml\" \"desired-path-to-junit-test-results.xml\"");
                return;
            }

            var xUnitTestResultsFilePath = args[0];
            var JUnitTestResultsFilePath = args[1];

            if (!File.Exists(xUnitTestResultsFilePath))
            {
                Console.WriteLine(
                    $"The xUnit test results file path provided \"{xUnitTestResultsFilePath}\" does not exist.");
                return;
            }

            var JUnitTestResultsDirectory = Path.GetDirectoryName(JUnitTestResultsFilePath);

            if (!Directory.Exists(JUnitTestResultsDirectory))
            {
                Directory.CreateDirectory(JUnitTestResultsDirectory);
            }
            
            var xlsTransform = new XslCompiledTransform();
            xlsTransform.Load($"{AppContext.BaseDirectory}/JUnit.xslt");

            var writerSettings = new XmlWriterSettings
            {
                OmitXmlDeclaration = false,
                Indent = true,
                Encoding = new UTF8Encoding(false)
            };

            using (var stream = new FileStream(JUnitTestResultsFilePath, FileMode.Create, FileAccess.Write))
            using (var results = XmlWriter.Create(stream, writerSettings))
            {
                xlsTransform.Transform(xUnitTestResultsFilePath, results);
            }

            Console.WriteLine(
                $"The xUnit test results file \"{xUnitTestResultsFilePath}\" has been converted to the JUnit test results file \"{JUnitTestResultsFilePath}\"");
        }
    }
}