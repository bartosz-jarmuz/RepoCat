// -----------------------------------------------------------------------
//  <copyright file="ManifestDeserializer.cs" company="bartosz.jarmuz@gmail.com">
//   Copyright (c) Bartosz Jarmuż. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Serialization;
using RepoCat.Transmission.Contracts;
using RepoCat.Transmission.Models;

namespace RepoCat.Serialization
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

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ProjectInfo), XmlNames.ProjectManifestNamespace);

            ProjectInfo info = (ProjectInfo)xmlSerializer.Deserialize(infoElement.CreateReader());
            
            LoadTags(infoElement, info.Tags);
            LoadProperties(infoElement, info.Properties);

            XElement componentsElement = infoElement.Element(XmlNames.GetComponentXName(XmlNames.Components));
            List<ComponentManifest> components = DeserializeComponents(componentsElement);

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
            List<ComponentManifest> list = new List<ComponentManifest>();
            if (componentsElement != null)
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(ComponentManifest), XmlNames.ProjectManifestNamespace);

                foreach (XElement xElement in componentsElement.Elements())
                {
                    ComponentManifest item = Deserialize(xmlSerializer, xElement);
                    list.Add(item);
                }
            }

            return list;
        }

        private static ComponentManifest Deserialize(XmlSerializer xmlSerializer, XElement xElement)
        {
            ComponentManifest item = (ComponentManifest)xmlSerializer.Deserialize(xElement.CreateReader());

            //do tags manually, bear in mind namespaces
            LoadTags(xElement, item.Tags);
            LoadProperties(xElement, item.Properties);
            return item;
        }

        private static void LoadProperties(XElement xElement, PropertiesCollection propertiesDictionary)
        {
            XElement parent = xElement.Elements().FirstOrDefault(x => x.Name.LocalName == XmlNames.Properties);
            if (parent != null)
            {
                IEnumerable<XElement> properties = parent.Elements();
                foreach (XElement property in properties)
                {
                    string key = property.Attribute(XmlNames.Key)?.Value;
                    if (!string.IsNullOrEmpty(key))
                    {
                        if (property.HasElements)
                        {
                            var valueElements = property.Elements().Where(x=>x.Name.LocalName == XmlNames.Value );
                            if (!propertiesDictionary.ContainsKey(key))
                            {
                                propertiesDictionary.Add(key, valueElements.Select(x => Json.Deserialize(x.Value)));
                            }
                        }
                        else
                        {
                            string value = property.Value;
                            if (!propertiesDictionary.ContainsKey(key))
                            {
                                propertiesDictionary.Add(key, Json.Deserialize(value));
                            }
                        }
                    }
                }
            }
        }

        private static void LoadTags(XElement xElement, List<string> tagsCollection)
        {
            string tags = xElement.Elements().FirstOrDefault(x => x.Name.LocalName == XmlNames.Tags)?.Attribute(XmlNames.Value)?.Value;
            if (tags != null)
            {
                string[] split = tags.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
                tagsCollection.AddRange(new List<string>(split));
            }
        }
    }
}