using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RepoCat.Portal.Models
{
    public class SearchIndexViewModel
    {
        public string Repository { get; set; } 
        public List<string> Repositories { get; set; } = new List<string>();
    }
}
