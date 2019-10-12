using System;
using System.Collections.Generic;

namespace RepoCat.Persistence.Models
{
    public class ManifestQueryResult
    {
        public List<ProjectInfo> Manifests { get; set; } = new List<ProjectInfo>();
        public TimeSpan Elapsed { get; set; }
        public string RepoStamp { get; set; }
        public string Repo { get; set; }
        public bool IsRegex { get; set; }

        public string QueryString { get; set; }
    }
}