using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RepoCat.RepositoryManagement.Service;
using RepoCat.Utilities;

namespace RepoCat.Portal.Areas.Catalog.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class ProjectsTableModel
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="projects"></param>
        /// <param name="isMultipleRepositories"></param>
        public ProjectsTableModel(List<ProjectInfoViewModel> projects, bool isMultipleRepositories)
        {
            this.Projects = projects;
            this.IsMultipleRepositories = isMultipleRepositories;
        }

        /// <summary>
        /// Gets or sets the manifests.
        /// </summary>
        /// <value>The manifests.</value>
        public List<ProjectInfoViewModel> Projects { get; internal set; } = new List<ProjectInfoViewModel>();
     
        /// <summary>
        /// Specifies whther the table contains projects from multiple repos
        /// </summary>
        public bool IsMultipleRepositories { get; set; }
    }
}