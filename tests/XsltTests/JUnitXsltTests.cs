using System;
using System.IO;
using System.Text;
using System.Xml;
using Xunit;

namespace XsltTests
{
    public class JUnitXsltTests
    {
        [Fact]
        public void GivenSingleAssemblyCollectionTest_WhenTransform_ThenExpectedJUnitFile()
        {
            // Arrange

            const string inputFileName = "single-assembly-collection-test";

            // Act

            var actual = JUnitXslt.Transform(inputFileName);

            // Assert

            var expected = GetExpected(inputFileName);

            Assert.Equal(expected, actual);
        }

        private static string GetExpected(string inputFileName)
        {
            return File.ReadAllText($"./expected/{inputFileName}.xml");
        }
    }
}