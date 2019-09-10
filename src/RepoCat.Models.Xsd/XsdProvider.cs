using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Schema;

namespace RepoCat.Models.Xsd
{
    public static class XsdProvider
    {
 
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

        public static XmlSchemaSet GetSchemaSet(SchemaNames names)
        {
            string text = GetSchemaText(names);
            var schema = XmlReader.Create(new StringReader(text));
            XmlSchemaSet schemas = new XmlSchemaSet();
            schemas.Add("", schema);
            return schemas;
        }



        private static string GetFileName(SchemaNames names)
        {
            switch (names)
            {
                case SchemaNames.ProjectManifestSchema:
                    return "ProjectManifestSchema.xsd";
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