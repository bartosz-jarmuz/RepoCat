using System;
using System.Collections.Generic;

namespace RepoCat.Portal.Models
{
    public class ProjectManifestViewModel
    {
        public string Repo { get; set; }
        public string RepoStamp { get; set; }
        public DateTime AddedDateTime { get; set; } = DateTime.UtcNow;
        public string AssemblyName { get; set; }
        public string ProjectPath { get; set; }
        public string ProjectName { get; set; }
        public string TargetExt { get; set; }
        public string OutputType { get; set; }
        public List<ComponentManifestViewModel> Components { get; set; } = new List<ComponentManifestViewModel>();
     

        public string GetAssemblyName()
        {
            if (!string.IsNullOrEmpty(this.AssemblyName))
            {
                return this.AssemblyName + "." + this.TargetExt.Trim('.');
            }

            return "";
        }

        public string GetIdentifier()
        {
            if (!string.IsNullOrEmpty(this.AssemblyName))
            {
                return (this.ProjectName + this.AssemblyName + this.TargetExt).Replace(".", "");
            }

            return "";
        }
    }
}