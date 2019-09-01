using System;
using System.IO;

namespace RepoCat.Models.ProjectInfo
{
    public class ProjectInfo
    {
        public string Repo { get; set; }
        public string RepoStamp { get; set; }
        public string RepoCatManifestPath { get; set; }
        public string AssemblyName { get; set; }
        public string ProjectPath { get; set; }
        public string TargetExt { get; set; }
        public string OutputType { get; set; }
        public string RepoCatManifest { get; set; }
        public string ProjectName { get; set; }
    }
}
