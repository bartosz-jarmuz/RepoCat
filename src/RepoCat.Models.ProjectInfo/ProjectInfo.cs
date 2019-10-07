using System;
using System.IO;

namespace RepoCat.Models.ProjectInfo
{
    public class ProjectInfo
    {
        public string ProjectName { get; set; }
        public string ProjectUri { get; set; }
        public string AssemblyName { get; set; }
        public string TargetExt { get; set; }
        public string OutputType { get; set; }
      
        public string RepositoryName { get; set; }
        /// <summary>
        /// A datetime or version stamp of the repository as of when the project info was read and transmitted
        /// </summary>
        public string RepositoryStamp { get; set; }
        public string ManifestPath { get; set; }
        public string ManifestContent { get; set; }
    }
}
