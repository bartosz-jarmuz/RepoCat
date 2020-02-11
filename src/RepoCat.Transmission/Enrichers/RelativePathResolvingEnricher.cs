// -----------------------------------------------------------------------
//  <copyright file="RelativePathResolvingEnricher.cs" company="bartosz.jarmuz@gmail.com">
//   Copyright (c) Bartosz Jarmuż. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using RepoCat.Transmission.Contracts;

namespace RepoCat.Transmission
{
    public class RelativePathResolvingEnricher : EnricherBase
    {

        public override void Enrich(XDocument manifestXmlDocument, string manifestFilePath, string inputUri)
        {
            //these are paths relative to the location of the manifest
            var elementsWithRelativePath = manifestXmlDocument?.Root?.Descendants()?.Attributes(XmlNames.IsRelativePath).Where(x => string.Equals(x.Value, "True", StringComparison.OrdinalIgnoreCase))??new List<XAttribute>();
            foreach (XAttribute isRelativePathAttribute in elementsWithRelativePath)
            {
                if (isRelativePathAttribute.Parent == null)
                {
                    throw new InvalidOperationException($"{isRelativePathAttribute} does not have a parent!");
                }
                var valueAttribute = isRelativePathAttribute.Parent.Attribute(XmlNames.Value);
                if (valueAttribute != null)
                {
                    //it's a custom property key
                    valueAttribute.Value = GetAbsolutePath(manifestFilePath, valueAttribute.Value);
                }
                else
                {
                    //its an element
                    isRelativePathAttribute.Parent.Value = GetAbsolutePath(manifestFilePath, isRelativePathAttribute.Parent.Value);
                }
            }
        }

        private static string GetAbsolutePath(string manifestFilePath, string valueAttribute)
        {
            if (valueAttribute.StartsWith("http") || valueAttribute.StartsWith("www."))
            {
                //assume its not actually a path
                return valueAttribute;
            }
            var basePath = Path.GetDirectoryName(manifestFilePath);
            if (basePath == null)
            {
                throw new InvalidOperationException($"Failed to find directory of {manifestFilePath}.");
            }
            var path = Path.Combine(basePath , valueAttribute.TrimStart(new []{'\\', '/'}));
            var fullPath = Path.GetFullPath(path);//resolve any \..\.. bits
            return fullPath;
        }
    }
}