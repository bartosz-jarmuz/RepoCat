using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.Schema;
using FluentAssertions;
using NUnit.Framework;
using RepoCat.Schemas;
using RepoCat.Transmission;

namespace RepoCat.Tests
{
    [TestFixture]
    public class SchemaProviderTests
    {

        [Test]
        public void TestProvider_Text_ProvidesEmbeddedSchema()
        {
            string schemaText = XsdProvider.GetSchemaText(SchemaName.ProjectInfo);

            schemaText.Should().NotBeNullOrWhiteSpace();
        }

        [Test]
        public void TestSchema_SampleFiles_AllAreValid()
        {
            Dictionary<string, List<string>> filesAndErrors = new Dictionary<string, List<string>>();
            IEnumerable<string> manifests = Directory
                .EnumerateFiles(Path.Combine(TestContext.CurrentContext.TestDirectory, "SampleManifestFiles"))
                .Concat(Directory.EnumerateFiles(
                    Path.Combine(TestContext.CurrentContext.TestDirectory, "SampleScriptsRepository"),
                    "*.*", SearchOption.AllDirectories).Where(x =>
                    x.EndsWith(Strings.ManifestSuffix, StringComparison.OrdinalIgnoreCase)));

            foreach (string manifestPath in manifests)
            {
                string text = File.ReadAllText(manifestPath);
                XDocument xDoc = XDocument.Parse(text);
                SchemaValidator validator = new SchemaValidator();
                List<string> errors = validator.ValidateManifest(xDoc);
                try
                {
                    var _ = ManifestDeserializer.DeserializeProjectInfo(xDoc.Root);
                }
                catch (Exception ex)
                {
                    errors.Add(ex.ToString());
                }

                filesAndErrors.Add(manifestPath, errors);
            }

            filesAndErrors.Count.Should().BeGreaterOrEqualTo(8);

            if (filesAndErrors.SelectMany(x=>x.Value).Any())
            {
                StringBuilder sb = new StringBuilder();
                foreach (KeyValuePair<string, List<string>> fileAndError in filesAndErrors)
                {
                    if (fileAndError.Value.Any())
                    {
                        sb.AppendLine($"File {fileAndError.Key} errors: \r\n\r\n{string.Join("\r\n", fileAndError.Value)}");
                    }
                }
                Assert.Fail(sb.ToString());
            }
        }

        [Test]
        public void TestSchema_RandomXml_IsNotValid()
        {
            string text = "<Project xmlns=\"https://git.io/RepoCat-ProjectInfo\">hi</Project>";
            var xDoc = XDocument.Parse(text);
           
            var validator = new SchemaValidator();
            var errors = validator.ValidateManifest(xDoc);

            errors.Should().NotBeNullOrEmpty("There should be errors in this file");
        }
    }


}