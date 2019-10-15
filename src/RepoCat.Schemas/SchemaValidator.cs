using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using System.Xml.Schema;

namespace RepoCat.Schemas
{
    /// <summary>
    /// Handles validations of XML files against schemas
    /// </summary>
    public class SchemaValidator
    {
        private Lazy<XmlSchemaSet> ComponentManifestSchemas { get; } = new Lazy<XmlSchemaSet>(()=> XsdProvider.GetSchemaSet(SchemaNames.ComponentManifest));

        /// <summary>
        /// Validates against ComponentManifest schema
        /// </summary>
        /// <param name="xDoc"></param>
        /// <returns></returns>
        public List<string> ValidateComponentManifest(XDocument xDoc)
        {
            return Validate(xDoc, ComponentManifestSchemas.Value);
        }

        /// <summary>
        /// Validates against ComponentManifest schema
        /// </summary>
        /// <param name="xmlString"></param>
        /// <param name="document"></param>
        /// <returns></returns>
        public List<string> ValidateComponentManifest(string xmlString, out XDocument document)
        {
            try
            {
                document = XDocument.Parse(xmlString);
            }
            catch (Exception ex)
            {
                document = null;
                return new List<string>()
                {
                    $"Invalid XML: {ex.Message}"
                };
            }

            return ValidateComponentManifest(document);
        }

        private List<string> Validate(XDocument xDoc, XmlSchemaSet schemas)
        {
            List<string> errors = new List<string>();
            xDoc.Validate(schemas, (sender, args) =>
            {
                errors.Add(args.Message);
            });
            return errors;
        }

    }
}
