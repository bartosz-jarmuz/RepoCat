using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml.Serialization;
using RepoCat.Transmission.Models;

namespace RepoCat.Transmission.Core
{
    /// <summary>
    /// Performs the deserialization of the component manifest
    /// </summary>
    public static class ManifestDeserializer
    {
        /// <summary>
        /// Loads the components from the specified manifest string
        /// </summary>
        /// <param name="manifest">The manifest.</param>
        /// <returns>List&lt;ComponentManifest&gt;.</returns>
        public static List<ComponentManifest> LoadComponents(string manifest)
        {
            var list = new List<ComponentManifest>();
            var xmlSerializer = new XmlSerializer(typeof(ComponentManifest));
            XDocument componentElements = XDocument.Parse(manifest);

            foreach (XElement xElement in componentElements.Root.Elements())
            {
                try
                {
                    ComponentManifest item = Deserialize(xmlSerializer, xElement);
                    list.Add(item);
                }
                catch (Exception)
                {
                    //todo
                }
            }

            return list;
        }

        private static ComponentManifest Deserialize(XmlSerializer xmlSerializer, XElement xElement)
        {
            var item = (ComponentManifest) xmlSerializer.Deserialize(xElement.CreateReader());

            //do tags manually
            LoadTags(xElement, item);
            LoadProperties(xElement, item);
            return item;
        }

        private static void LoadProperties(XElement xElement, ComponentManifest item)
        {
            var parent = xElement.Element("Properties");
            item.Properties = new Dictionary<string, string>();
            if (parent != null)
            {
                var properties = parent.Elements();
                foreach (XElement property in properties)
                {
                    var key = property.Attribute("Key")?.Value;
                    var value = property.Attribute("Value")?.Value;
                    if (!string.IsNullOrEmpty(key))
                    {
                        if (!item.Properties.ContainsKey(key))
                        {
                            item.Properties.Add(key, value);
                        }
                    }
                }
            }
        }

        private static void LoadTags(XElement xElement, ComponentManifest item)
        {
            var tags = xElement.Element("Tags")?.Attribute("Values")?.Value;
            if (tags != null)
            {
                var split = tags.Split(new[] {',', ';'}, StringSplitOptions.RemoveEmptyEntries);
                item.Tags = new List<string>(split);
            }
        }
    }
}
