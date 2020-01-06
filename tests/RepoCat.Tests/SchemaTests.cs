using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using System.Xml.Schema;
using FluentAssertions;
using NUnit.Framework;
using RepoCat.Schemas;

namespace RepoCat.Tests
{
    public class SchemaProviderTests
    {

        [Test]
        public void TestProvider_Text_ProvidesEmbeddedSchema()
        {
            string schemaText = XsdProvider.GetSchemaText(SchemaName.Components);

            schemaText.Should().NotBeNullOrWhiteSpace();
        }

        [Test]
        public void TestSchema_SampleFile_IsValid()
        {
            string text = File.ReadAllText(TestContext.CurrentContext.TestDirectory + "\\SampleManifestFiles\\SampleManifest.RepoCat.xml");
            var xDoc = XDocument.Parse(text);
            var validator = new SchemaValidator();
            var errors = validator.ValidateComponentManifest(xDoc);

            errors.Should().BeEmpty("There should be no errors in this file");
        }

        [Test]
        public void TestSchema_RandomXml_IsNotValid()
        {
            string text = "<Project xmlns=\"https://git.io/RepoCat-Components\">hi</Project>";
            var xDoc = XDocument.Parse(text);
           
            var validator = new SchemaValidator();
            var errors = validator.ValidateComponentManifest(xDoc);

            errors.Should().NotBeNullOrEmpty("There should be errors in this file");
        }
    }


}