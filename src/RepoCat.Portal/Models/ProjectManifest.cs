using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RepoCat.Portal.Models
{
    public class ProjectManifest
    {
        public string AssemblyName { get; set; }
        public string ProjectPath { get; set; }
        public string TargetExt { get; set; }
        public string OutputType { get; set; }
    }
}
