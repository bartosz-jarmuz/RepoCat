using System.Collections.Generic;

namespace RepoCat.Portal.Models
{
    public class SearchIndexViewModel
    {
        public string Repository { get; set; } 
        public List<string> Repositories { get; set; } = new List<string>();
        public ManifestQueryResultViewModel Result { get; set; }
        public string Query { get; set; }
        public bool IsRegex { get; set; }
    }
}
