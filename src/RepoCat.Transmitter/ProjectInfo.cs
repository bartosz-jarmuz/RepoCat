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
    }

    public class Manifest
    {
        public XDocument
    }
}