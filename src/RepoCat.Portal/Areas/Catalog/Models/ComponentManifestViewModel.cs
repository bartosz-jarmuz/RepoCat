using System.Collections.Generic;

namespace RepoCat.Portal.Models
{
    public class ComponentManifestViewModel
    {
        public string Name { get; set; }
        public string ContactEmail { get; set; }
        public string Description { get; set; }
        public string DocumentationUri { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
        public Dictionary<string, string> Properties { get; set; } = new Dictionary<string, string>();
    }
}