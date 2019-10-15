using System;
using System.IO;
using System.Linq;
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
        /// <param name="names">The names.</param>
        /// <returns>System.String.</returns>
        public static string GetSchemaText(SchemaNames names)
        {
            string resourceName = GetFileName(names);
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
        /// <param name="names">The names.</param>
        /// <returns>XmlSchemaSet.</returns>
        public static XmlSchemaSet GetSchemaSet(SchemaNames names)
        {
            string text = GetSchemaText(names);
            var xml = XDocument.Parse(text);
            string nameSpace = xml?.Root?.Attribute("targetNamespace")?.Value;

            var schema = XmlReader.Create(new StringReader(text));
            XmlSchemaSet schemas = new XmlSchemaSet();
            schemas.Add(nameSpace, schema);
            return schemas;
        }

        private static string GetFileName(SchemaNames names)
        {
            switch (names)
            {
                case SchemaNames.Components:
                    return "Components.xsd";
                default:
                    throw new ArgumentOutOfRangeException(nameof(names), names, null);
            }
        }

        private static string FormatResourceName(Assembly assembly, string resourceName)
        {
            return assembly.GetName().Name + "." + resourceName.Replace(" ", "_")
                       .Replace("\\", ".")
                       .Replace("/", ".");
        }
    }
}