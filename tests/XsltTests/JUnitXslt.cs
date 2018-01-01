using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Xsl;

namespace XsltTests
{
    public static class JUnitXslt
    {
        private static readonly XslCompiledTransform XslTransform;
        private static readonly XmlWriterSettings WriterSettings;

        static JUnitXslt()
        {
            XslTransform = new XslCompiledTransform();
            XslTransform.Load("JUnit.xslt");

            WriterSettings = new XmlWriterSettings
            {
                OmitXmlDeclaration = false,
                Indent = true,
                Encoding = new UTF8Encoding(false)
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputFileName">If you want to transform `./input/single-assembly-collection-test.xml`
        /// Use `single-assembly-collection-test`</param>
        /// <returns></returns>
        public static string Transform(string inputFileName)
        {
            using (var stream = new MemoryStream())
            using (var results = XmlWriter.Create(stream, WriterSettings))
            {
                XslTransform.Transform($"./input/{inputFileName}.xml", results);
                return Encoding.UTF8
                    .GetString(stream.ToArray());
            }
        }
    }
}