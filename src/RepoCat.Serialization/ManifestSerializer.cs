// -----------------------------------------------------------------------
//  <copyright file="ManifestSerializer.cs" company="bartosz.jarmuz@gmail.com">
//   Copyright (c) Bartosz Jarmuż. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using System.Xml.Serialization;
using RepoCat.Transmission.Contracts;
using RepoCat.Transmission.Models;

namespace RepoCat.Serialization
{
    /// <summary>
    /// Performs the deserialization of the component manifest
    /// </summary>
    public static class ManifestSerializer
    {
        /// <summary>
        /// Serializes ProjectInfo as XML
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public static XElement SerializeProjectInfo(ProjectInfo info)
        {
            if (info == null) throw new ArgumentNullException(nameof(info));

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ProjectInfo), XmlNames.ProjectManifestNamespace);
            using (StringWriter sw = new StringWriter())
            {
                xmlSerializer.Serialize(sw, info);
                XElement projectInfoElement = XElement.Parse(sw.ToString());
                //do tags manually, bear in mind namespaces
                SaveTags(info.Tags, projectInfoElement);
                SaveProperties(info.Properties, projectInfoElement);

                var componentsElement = SerializeComponents(info.Components);

                projectInfoElement.Add(componentsElement);
                var final = ClearNamespaces(projectInfoElement);
                return final;
            }
        }

        /// <summary>
        /// Stores the list of component manifests as an XML element
        /// </summary>
        /// <param name="manifests"></param>
        /// <returns></returns>
        public static XElement SerializeComponents(IEnumerable<ComponentManifest> manifests)
        {
            if (manifests == null) throw new ArgumentNullException(nameof(manifests));

            List<XElement> list = new List<XElement>();
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ComponentManifest), XmlNames.ProjectManifestNamespace);

            foreach (ComponentManifest manifest in manifests)
            {
                XElement item = SerializeComponent(xmlSerializer, manifest);
                list.Add(item);
            }

            XElement components = new XElement(XmlNames.GetComponentXName(XmlNames.Components));

            foreach (XElement xElement in list)
            {
                components.Add(xElement);
            }

            return components;
        }

        private static XElement ClearNamespaces(XElement final)
        {
            TryAddNamespace(final, "xsi", @"http://www.w3.org/2001/XMLSchema-instance");
            TryAddNamespace(final, "xsd", @"http://www.w3.org/2001/XMLSchema");

            using (MemoryStream s = new MemoryStream())
            {
                final.Save(s, SaveOptions.OmitDuplicateNamespaces);
                s.Flush();
                s.Position = 0;
                XElement ele = XElement.Load(s);
                return ele;
            }
        }

        private static void TryAddNamespace(XElement element, string prefix, string namespaceName)
        {
            try
            {
                element.Add(new XAttribute(XNamespace.Xmlns + prefix, namespaceName));
            }
            catch (Exception)
            {
                //ok, it's there already
            }
        }

        private static XElement SerializeComponent(XmlSerializer xmlSerializer, ComponentManifest manifest)
        {
            using (StringWriter sw = new StringWriter())
            {
                xmlSerializer.Serialize(sw, manifest);
                XElement element = XElement.Parse(sw.ToString());
                //do tags manually, bear in mind namespaces
                SaveTags(manifest.Tags, element);
                SaveProperties(manifest.Properties, element);
              
                return element;
            }
        }


        private static void SaveProperties(PropertiesCollection properties, XElement element)
        {
            XElement props = new XElement(XmlNames.GetComponentXName(XmlNames.Properties));

            if (properties != null)
            {
                foreach (var manifestProperty in properties)
                {
                    XElement propertyElement = new XElement(XmlNames.GetComponentXName(XmlNames.Property));
                    propertyElement.Add(new XAttribute(XmlNames.Key, manifestProperty.Key));
                    propertyElement.Value =Json.Serialize(manifestProperty.Value);
                    props.Add(propertyElement);
                }
            }

            element.Add(props);
        }

        private static void SaveTags(List<string> tags, XElement componentElement)
        {
            string tagsString = string.Join(";", tags);
            XElement tagsElement = new XElement(XmlNames.GetComponentXName(XmlNames.Tags));
            tagsElement.Add(new XAttribute(XmlNames.Value, tagsString));
            componentElement.Add(tagsElement);
        }
    }
}
