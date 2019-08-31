﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace RepoCat.Portal.Models
{
    [XmlRoot("Component")]
    public class ComponentManifest
    {
        public string Name { get; set; }
        public string ContactEmail { get; set; }
        public string Description { get; set; }
        public string DocumentationUri { get; set; }
        [XmlIgnore]
        public List<string> Tags { get; set; } = new List<string>();
        [XmlIgnore]
        public Dictionary<string, string> Properties { get; set; } = new Dictionary<string, string>();
    }
}