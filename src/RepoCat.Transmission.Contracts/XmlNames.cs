// -----------------------------------------------------------------------
//  <copyright file="XmlNames.cs" company="bartosz.jarmuz@gmail.com">
//   Copyright (c) Bartosz Jarmuż. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Xml.Linq;

namespace RepoCat.Transmission.Contracts
{
    /// <summary>
    /// Strong typing for serialization
    /// </summary>
    public static class XmlNames
    {
        /// <summary>
        /// Gets the XName, appending the namespace
        /// </summary>
        /// <param name="localName"></param>
        /// <returns></returns>
        public static XName GetComponentXName(string localName)
        {
            return XName.Get(localName, ProjectManifestNamespace);
        }

        /// <summary>
        /// The namespace of the component manifests
        /// </summary>
        public const string ProjectManifestNamespace = "https://git.io/RepoCat-ProjectInfo";

        /// <summary>
        /// Tags of a component
        /// </summary>
        public static string Tags { get; } = "Tags";


        /// <summary>
        /// Properties of a component
        /// </summary>
        public static string Properties { get; } = "Properties";

        /// <summary>
        /// Component property element name
        /// </summary>
        public static string Add { get; } = "Add";

        /// <summary>
        /// 
        /// </summary>
        public static string IsRelativePath { get; } = "IsRelativePath";

        /// <summary>
        /// Value element
        /// </summary>
        public static string Value { get; } = "Value";

       
        /// <summary>
        /// Property key 
        /// </summary>
        public static string Key { get; } = "Key";

        /// <summary>
        /// Components collection
        /// </summary>
        public static string Components { get; } = "Components";
    }
}