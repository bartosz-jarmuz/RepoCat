using System;
using System.Collections.Generic;
using RepoCat.Models.Manifests;

namespace RepoCat.Web.Persistence
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