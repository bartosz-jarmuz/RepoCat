using System;
using System.Collections.Generic;

namespace RepoCat.Portal.Models.Domain
{
    public class ManifestQueryResult
    {
        public List<ProjectManifest> Manifests { get; set; } = new List<ProjectManifest>();
        public TimeSpan Elapsed { get; set; }
        public string RepoStamp { get; set; }
        public string Repo { get; set; }
        public bool IsRegex { get; set; }

        public string QueryString { get; set; }
    }
}