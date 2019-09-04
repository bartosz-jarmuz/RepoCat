using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RepoCat.Portal.Areas.Catalog.Models
{
    public class TagBadgeViewModel
    {
        public TagBadgeViewModel(string repoName, string tagText)
        {
            this.RepoName = repoName;
            this.TagText = tagText;
        }

        public string RepoName { get; set; }

        public string TagText { get; set; }
    }
}
