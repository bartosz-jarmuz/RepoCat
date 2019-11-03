using System.Collections.Generic;

namespace RepoCat.ProjectFileReaders
{
    public class Project
    {
        public string Name { get; set; }
        public string FullPath { get; set; }
        public string DirectoryPath { get; set; }
        public string AssemblyName { get; set; }
        public string OutputType { get; set; }
        public string TargetFramework { get; set; }
        public string TargetExtension { get; set; }

        public List<ProjectItem> Items { get; } = new List<ProjectItem>();

    }


}