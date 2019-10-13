using System;
using System.Collections.Generic;

namespace RepoCat.Portal.Areas.Catalog.Models
{

    /// <summary>
    /// Class ProjectInfoViewModel.
    /// </summary>
    public class ProjectInfoViewModel
    {
        /// <summary>
        /// Gets or sets the name of the repository.
        /// </summary>
        /// <value>The name of the repository.</value>
        public string RepositoryName { get; set; }
        /// <summary>
        /// Gets or sets the repository stamp.
        /// </summary>
        /// <value>The repository stamp.</value>
        public string RepositoryStamp { get; set; }
        /// <summary>
        /// Gets or sets the added date time.
        /// </summary>
        /// <value>The added date time.</value>
        public DateTime AddedDateTime { get; set; } = DateTime.UtcNow;
        /// <summary>
        /// Gets or sets the name of the assembly.
        /// </summary>
        /// <value>The name of the assembly.</value>
        public string AssemblyName { get; set; }
        /// <summary>
        /// Gets or sets the project URI.
        /// </summary>
        /// <value>The project URI.</value>
        public string ProjectUri { get; set; }
        /// <summary>
        /// Gets or sets the name of the project.
        /// </summary>
        /// <value>The name of the project.</value>
        public string ProjectName { get; set; }
        /// <summary>
        /// Gets or sets the target ext.
        /// </summary>
        /// <value>The target ext.</value>
        public string TargetExt { get; set; }
        /// <summary>
        /// Gets or sets the type of the output.
        /// </summary>
        /// <value>The type of the output.</value>
        public string OutputType { get; set; }
        /// <summary>
        /// Gets or sets the components.
        /// </summary>
        /// <value>The components.</value>
        public List<ComponentManifestViewModel> Components { get; set; } = new List<ComponentManifestViewModel>();

        /// <summary>
        /// Gets the name of the assembly.
        /// </summary>
        /// <returns>System.String.</returns>
        public string GetAssemblyName()
        {
            if (!string.IsNullOrEmpty(this.AssemblyName))
            {
                return this.AssemblyName + "." + this.TargetExt?.Trim('.');
            }

            return "";
        }

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <returns>System.String.</returns>
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