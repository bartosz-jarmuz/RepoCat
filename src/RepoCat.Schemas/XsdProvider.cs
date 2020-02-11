// -----------------------------------------------------------------------
//  <copyright file="XsdProvider.cs" company="bartosz.jarmuz@gmail.com">
//   Copyright (c) Bartosz Jarmuż. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace RepoCat.Schemas
{
    /// <summary>
    /// Provides schemas
    /// </summary>
    public static class XsdProvider
    {
        /// <summary>
        /// Gets the schema text.
        /// </summary>
        /// <param name="name">The names.</param>
        /// <returns>System.String.</returns>
        public static string GetSchemaText(SchemaName name)
        {
            string resourceName = GetFileName(name);
            string formattedResourceName = FormatResourceName(typeof(XsdProvider).Assembly, resourceName);
            using (Stream resourceStream =
                typeof(XsdProvider).Assembly.GetManifestResourceStream(formattedResourceName))
            {
                if (resourceStream == null)
                    return null;

                using (StreamReader reader = new StreamReader(resourceStream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        /// <summary>
        /// Gets the schema set.
        /// </summary>
        /// <param name="name">The names.</param>
        /// <returns>XmlSchemaSet.</returns>
        public static XmlSchemaSet GetSchemaSet(SchemaName name)
        {
            string text = GetSchemaText(name);
            var xml = XDocument.Parse(text);
            string nameSpace = xml?.Root?.Attribute("targetNamespace")?.Value;

            using (XmlReader schema = XmlReader.Create(new StringReader(text)))
            {
                XmlSchemaSet schemas = new XmlSchemaSet();
                schemas.Add(nameSpace, schema);
                return schemas;
            }
            
        }

        private static string GetFileName(SchemaName name)
        {
            switch (name)
            {
                case SchemaName.ProjectInfo:
                    return "ProjectInfo.xsd";
                default:
                    throw new ArgumentOutOfRangeException(nameof(name), name, null);
            }
        }

        private static string FormatResourceName(Assembly assembly, string resourceName)
        {
            //the name must include folder/subfolder paths!
            return assembly.GetName().Name + ".XSD." + resourceName.Replace(" ", "_")
                       .Replace("\\", ".")
                       .Replace("/", ".");
        }
    }
}