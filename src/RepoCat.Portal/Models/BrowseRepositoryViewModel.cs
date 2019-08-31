using System;
using System.Collections.Generic;

namespace RepoCat.Portal.Models
{
    public class BrowseRepositoryViewModel
    {
        public string RepoName { get; set; }
        public string RepoStamp { get; set; }
        public int NumberOfProjects { get; set; }
        public DateTime ImportedDate { get; set; }
        public TimeSpan ImportDuration { get; set; }
        public List<ProjectManifestViewModel> ProjectManifestViewModel { get; set; }
        public int NumberOfComponents { get; set; }
        public int NumberOfTags { get; set; }
    }
}