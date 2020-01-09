using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Serialization;
using RepoCat.Transmission.Models;

namespace RepoCat.Transmission.Client
{
    /// <summary>
    /// Performs the deserialization of the component manifest
    /// </summary>
    public static class ManifestDeserializer
    {
        /// <summary>
        /// Serializes ProjectInfo as XML
        /// </summary>
        /// <param name="infoElement"></param>
        /// <returns></returns>
        public static ProjectInfo DeserializeProjectInfo(XElement infoElement)
        {
            if (infoElement == null) throw new ArgumentNullException(nameof(infoElement));

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ProjectInfo));
            var info = (ProjectInfo)xmlSerializer.Deserialize(infoElement.CreateReader());

            var components = DeserializeComponents(infoElement.Element(XmlNames.GetComponentXName(XmlNames.Components)));

            info.Components.AddRange(components);
            return info;
        }

        /// <summary>
        /// Loads the components from the specified manifest string
        /// </summary>
        /// <param name="componentsElement">The manifest.</param>
        /// <returns>List&lt;ComponentManifest&gt;.</returns>
        public static List<ComponentManifest> DeserializeComponents(XElement componentsElement)
        {
            var list = new List<ComponentManifest>();
            if (componentsElement != null)
            {
                var xmlSerializer = new XmlSerializer(typeof(ComponentManifest), XmlNames.ComponentManifestNamespace);

                foreach (XElement xElement in componentsElement.Elements())
                {
                    ComponentManifest item = Deserialize(xmlSerializer, xElement);
                    list.Add(item);
                }
            }

            return list;
        }

        /// <summary>
        /// Loads the components from the specified manifest string
        /// </summary>
        /// <param name="manifest">The manifest.</param>
        /// <returns>List&lt;ComponentManifest&gt;.</returns>
        public static List<ComponentManifest> DeserializeComponents(string manifest)
        {
            XElement componentsElement = XElement.Parse(manifest);
            return DeserializeComponents(componentsElement);
        }

        private static ComponentManifest Deserialize(XmlSerializer xmlSerializer, XElement xElement)
        {
            var item = (ComponentManifest)xmlSerializer.Deserialize(xElement.CreateReader());

            //do tags manually, bear in mind namespaces
            LoadTags(xElement, item);
            LoadProperties(xElement, item);
            return item;
        }

        private static void LoadProperties(XElement xElement, ComponentManifest item)
        {
            var parent = xElement.Elements().FirstOrDefault(x => x.Name.LocalName == XmlNames.Properties);
            if (parent != null)
            {
                var properties = parent.Elements();
                foreach (XElement property in properties)
                {
                    var key = property.Attribute(XmlNames.Key)?.Value;
                    var value = property.Attribute(XmlNames.Value)?.Value;
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
            var tags = xElement.Elements().FirstOrDefault(x => x.Name.LocalName == XmlNames.Tags)?.Attribute(XmlNames.Value)?.Value;
            if (tags != null)
            {
                var split = tags.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
                item.Tags.AddRange(new List<string>(split));
            }
        }
    }
}