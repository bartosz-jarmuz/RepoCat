using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Microsoft.Build.Experimental.Graph;
using RepoCat.Transmission.Models;

namespace RepoCat.Transmission.Core
{
    /// <summary>
    /// Performs the deserialization of the component manifest
    /// </summary>
    public static class ManifestSerializer
    {

        
     

        public static XElement SerializeComponents(List<ComponentManifest> manifests)
        {
            var list = new List<XElement>();
            var xmlSerializer = new XmlSerializer(typeof(ComponentManifest), XmlNames.ComponentManifestNamespace);

            foreach (var manifest in manifests)
            {
                try
                {
                    XElement item = Serialize(xmlSerializer, manifest);
                    list.Add(item);
                }
                catch (Exception)
                {
                    throw;
                    //todo
                }
            }

            var final = new XElement(XName.Get(XmlNames.Components, XmlNames.ComponentManifestNamespace), list);
            return final;
        }

      

        private static XElement Serialize(XmlSerializer xmlSerializer, ComponentManifest manifest)
        {
            using (var sw = new StringWriter())
            {
                XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                ns.Add("", XmlNames.ComponentManifestNamespace);

                xmlSerializer.Serialize(sw, manifest, ns);
                XElement element = XElement.Parse(sw.ToString());
                //do tags manually, bear in mind namespaces
                SaveTags(manifest, element);
                SaveProperties(manifest, element);

              
                return element;
            }
        }

        private static void SaveProperties(ComponentManifest manifest, XElement element)
        {
            var props = new XElement(XName.Get(nameof(ComponentManifest.Properties), XmlNames.ComponentManifestNamespace));

            if (manifest.Properties != null)
            {
                foreach (KeyValuePair<string, string> manifestProperty in manifest.Properties)
                {
                    var propertyElement = new XElement(XmlNames.Add);
                    propertyElement.Add(new XAttribute(XmlNames.Key, manifestProperty.Key));
                    propertyElement.Add(new XAttribute(XmlNames.Value, manifestProperty.Value));
                    props.Add(propertyElement);
                }
            }

            element.Add(props);
        }

        private static void SaveTags(ComponentManifest manifest, XElement componentElement)
        {
            string tags = string.Join(";", manifest.Tags);
            var tagsElement = new XElement(XName.Get(nameof(ComponentManifest.Tags), XmlNames.ComponentManifestNamespace), tags);

            componentElement.Add(tagsElement);

        }

        /// <summary>
        /// Loads the components from the specified manifest string
        /// </summary>
        /// <param name="manifest">The manifest.</param>
        /// <returns>List&lt;ComponentManifest&gt;.</returns>
        public static List<ComponentManifest> DeserializeComponents(string manifest)
        {
            var list = new List<ComponentManifest>();

            var xmlSerializer = new XmlSerializer(typeof(ComponentManifest), XmlNames.ComponentManifestNamespace);
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
                    throw;
                    //todo
                }
            }
            return list;
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
            item.Properties = new Dictionary<string, string>();
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
            var tags = xElement.Elements().FirstOrDefault(x=>x.Name.LocalName == XmlNames.Tags)?.Attribute(XmlNames.Value)?.Value;
            if (tags != null)
            {
                var split = tags.Split(new[] {',', ';'}, StringSplitOptions.RemoveEmptyEntries);
                item.Tags = new List<string>(split);
            }
        }
    }
}
