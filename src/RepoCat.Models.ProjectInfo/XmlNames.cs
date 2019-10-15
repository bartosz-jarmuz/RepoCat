using System.Xml.Linq;

namespace RepoCat.Transmission.Models
{
    /// <summary>
    /// Strong typing for serialization
    /// </summary>
    public static class XmlNames
    {
        /// <summary>
        /// The namespace of the component manifests
        /// </summary>
        public const string ComponentManifestNamespace = "https://github.com/bartosz-jarmuz/RepoCat-ComponentManifest";

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