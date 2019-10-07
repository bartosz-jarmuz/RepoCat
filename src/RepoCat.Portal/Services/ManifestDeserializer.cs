using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml.Serialization;
using RepoCat.Persistence.Models;
using RepoCat.Portal.Models;

namespace RepoCat.Portal.Services
{
    public static class ManifestDeserializer
    {
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
            var tags = xElement.Element("Tags")?.Attribute("Values")?.Value;
            if (tags != null)
            {
                var split = tags.Split(new[] {',', ';'}, StringSplitOptions.RemoveEmptyEntries);
                item.Tags = new List<string>(split);
            }
            return item;
        }
    }
}
