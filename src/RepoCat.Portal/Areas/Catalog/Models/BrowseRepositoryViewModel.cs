using System;
using System.Collections.Generic;
using RepoCat.Persistence.Models;

namespace RepoCat.Portal.Areas.Catalog.Models
{
    /// <summary>
    /// Class BrowseRepositoryViewModel.
    /// </summary>
    public class BrowseRepositoryViewModel
    {
        /// <summary>
        /// Gets or sets the name of the repository.
        /// </summary>
        /// <value>The name of the repository.</value>
        public string RepositoryName { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string RepositoryMode { get; set; }
        /// <summary>
        /// Gets or sets the repository stamp.
        /// </summary>
        /// <value>The repository stamp.</value>
        public string RepositoryStamp { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<string> RepositoryStamps { get; set; } = new List<string>();
        /// <summary>
        /// Gets or sets the number of projects.
        /// </summary>
        /// <value>The number of projects.</value>
        public int NumberOfProjects { get; set; }
        /// <summary>
        /// Gets or sets the imported date.
        /// </summary>
        /// <value>The imported date.</value>
        public DateTime ImportedDate { get; set; }
        /// <summary>
        /// Gets or sets the duration of the import.
        /// </summary>
        /// <value>The duration of the import.</value>
        public TimeSpan ImportDuration { get; set; }
        /// <summary>
        /// Gets or sets the project manifest view models.
        /// </summary>
        /// <value>The project manifest view models.</value>
        public List<ProjectInfoViewModel> ProjectManifestViewModels { get; internal set; } = new List<ProjectInfoViewModel>();
        /// <summary>
        /// Gets or sets the number of components.
        /// </summary>
        /// <value>The number of components.</value>
        public int NumberOfComponents { get; set; }
        /// <summary>
        /// Gets or sets the number of tags.
        /// </summary>
        /// <value>The number of tags.</value>
        public int NumberOfTags { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public int NumberOfProperties { get; set; }

        /// <summary>
        /// Number of repository stamps in this repo
        /// </summary>
        public int NumberOfStamps { get; set; }
    }
}