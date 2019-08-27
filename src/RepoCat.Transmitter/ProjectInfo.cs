using System.IO;
using System.Xml.Linq;

namespace RepoCat.Transmitter
{
    public class ProjectInfo
    {
        public string RepoCatManifestPath { get; set; }
        public string AssemblyName { get; set; }
        public string ProjectPath { get; set; }
        public string TargetExt { get; set; }
        public string OutputType { get; set; }
        public string RepoCatManifest { get; set; }
        public string GetName() => Path.GetFileName(ProjectPath);

    }
}