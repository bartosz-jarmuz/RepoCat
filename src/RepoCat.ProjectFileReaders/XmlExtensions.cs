﻿using System.Linq;
using System.Xml.Linq;

namespace RepoCat.ProjectFileReaders
{
    internal static class XmlExtensions
    {
        public static XElement GetByLocalName(this XElement element, string localName)
        {
            return element?.Elements().FirstOrDefault(x => x.Name.LocalName == localName);
        }
    }
}