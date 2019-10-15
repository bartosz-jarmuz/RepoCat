using System.Collections.Generic;
using System.Xml.Serialization;

namespace RepoCat.Transmission.Models
{
    /// <summary>
    /// Component manifests contains a human-created short metadata about a top-level functional component in an assembly.
    /// <para>A component is 'something of interest for a user' a tool, an app (or significant part of it), a plugin etc. </para>
    /// </summary>
    [XmlRoot("Component")]
    public class ComponentManifest
    {
        /// <summary>
        /// Gets or sets the name of the component
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the short description of the component.
        /// </summary>
        /// <value>The description.</value>
        public string Description { get; set; }
        /// <summary>
        /// Gets or sets the component documentation URI.
        /// </summary>
        /// <value>The documentation URI.</value>
        public string DocumentationUri { get; set; }
        /// <summary>
        /// Gets or sets the tags that should allow for a component to be found in a repository catalog.
        /// </summary>
        /// <value>The tags.</value>
        [XmlIgnore]
        public List<string> Tags { get; set; } = new List<string>();
        /// <summary>
        /// Gets or sets the additional key-value properties associated with a component.
        /// These properties might be enriched by a transmitter plugin automatically
        /// (e.g. if a transmitter plugin uses reflection to scan through code for some extra info)
        /// </summary>
        /// <value>The properties.</value>
        [XmlIgnore]
        public Dictionary<string, string> Properties { get; set; } = new Dictionary<string, string>();
    }
}
