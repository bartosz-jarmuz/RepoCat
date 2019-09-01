using System;
using System.Collections.Generic;
using RepoCat.Portal.Models.Domain;

namespace RepoCat.Portal.Models
{
    public class ManifestQueryResultViewModel
    {
        public List<ProjectManifestViewModel> Manifests { get; set; } = new List<ProjectManifestViewModel>();
        public TimeSpan Elapsed { get; set; }
        public string RepoStamp { get; set; }
    }
}