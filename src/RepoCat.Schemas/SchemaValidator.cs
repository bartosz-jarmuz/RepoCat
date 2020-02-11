// -----------------------------------------------------------------------
//  <copyright file="SchemaValidator.cs" company="bartosz.jarmuz@gmail.com">
//   Copyright (c) Bartosz Jarmuż. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml.Schema;

namespace RepoCat.Schemas
{
    /// <summary>
    /// Handles validations of XML files against schemas
    /// </summary>
    public class SchemaValidator
    {
        private Lazy<XmlSchemaSet> ComponentManifestSchemas { get; } = new Lazy<XmlSchemaSet>(()=> XsdProvider.GetSchemaSet(SchemaName.ProjectInfo));

        /// <summary>
        /// Validates against ComponentManifest schema
        /// </summary>
        /// <param name="xDoc"></param>
        /// <returns></returns>
        public List<string> ValidateManifest(XDocument xDoc)
        {
            return Validate(xDoc, this.ComponentManifestSchemas.Value);
        }

        /// <summary>
        /// Validates against ComponentManifest schema
        /// </summary>
        /// <param name="xmlString"></param>
        /// <param name="document"></param>
        /// <returns></returns>
        public List<string> ValidateManifest(string xmlString, out XDocument document)
        {
            if (xmlString == null) throw new ArgumentNullException(nameof(xmlString));
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

            return this.ValidateManifest(document);
        }

        private static List<string> Validate(XDocument xDoc, XmlSchemaSet schemas)
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
