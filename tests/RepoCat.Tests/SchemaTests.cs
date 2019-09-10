using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using FluentAssertions;
using NUnit.Framework;
using RepoCat.Models.Xsd;
using RepoCat.Portal.Models;
using RepoCat.Portal.Services;

namespace Tests
{
    public class SchemaProviderTests
    {

        [Test]
        public void TestProvider_Text_ProvidesEmbeddedSchema()
        {
            var schemaText = XsdProvider.GetSchemaText(SchemaNames.ProjectManifestSchema);

            schemaText.Should().NotBeNullOrWhiteSpace();
        }

        [Test]
        public void TestSchema_SampleFile_IsValid()
        {
            string text = File.ReadAllText(TestContext.CurrentContext.TestDirectory + "\\TestFiles\\ComponentsOnlyManifest.RepoCat.xml");
            var xDoc = XDocument.Parse(text);

            var schemas = XsdProvider.GetSchemaSet(SchemaNames.ProjectManifestSchema );
            

            var errors = new List<string>();
            xDoc.Validate(schemas, (sender, args) =>
            {
                errors.Add(args.Message);
            });

            errors.Should().BeEmpty("There should be no errors in this file");
        }

        [Test]
        public void TestSchema_RandomXml_IsNotValid()
        {
            string text = "<Project>hi</Project>";
            var xDoc = XDocument.Parse(text);

            var schemas = XsdProvider.GetSchemaSet(SchemaNames.ProjectManifestSchema);


            var errors = new List<string>();
            xDoc.Validate(schemas, (sender, args) =>
            {
                errors.Add(args.Message);
            });

            errors.Should().NotBeNullOrEmpty("There should be errors in this file");
        }
    }


}