﻿using System;
using System.Collections.Generic;
using System.Globalization;

namespace RepoCat.Portal.Areas.Catalog.Models
{

    /// <summary>
    /// Class ProjectInfoViewModel.
    /// </summary>
    public class ProjectInfoViewModel
    {
        /// <summary>
        /// The Id of the project
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// When queries are run on many repositories, add a repo badge to each project 
        /// </summary>
        public bool DisplayRepositoryName { get; set; }

        /// <summary>
        /// Gets or sets the name of the repository.
        /// </summary>
        /// <value>The name of the repository.</value>
        public string RepositoryName { get; set; }

        /// <summary>
        /// Set to true if the project info was generated automatically, without a human-created manifest file
        /// </summary>
        public bool Autogenerated { get; set; }

        /// <summary>
        /// Name of the organization in which repo is
        /// </summary>
        public string OrganizationName { get; set; }
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
        /// Who maintains the project at the moment
        /// </summary>
        public string Owner { get; set; }

        /// <summary>
        /// A description of problem
        /// </summary>
        public string ProjectDescription { get; set; }
        /// <summary>
        /// Gets or sets the target ext.
        /// </summary>
        /// <value>The target ext.</value>
        public string TargetExtension { get; set; }
        /// <summary>
        /// Gets or sets the type of the output.
        /// </summary>
        /// <value>The type of the output.</value>
        public string OutputType { get; set; }

        /// <summary>
        /// Gets or sets the tags that should allow for a project to be found in a repository catalog.
        /// These are properties applicable to entire project, not specific components
        /// </summary>
        /// <value>The tags.</value>
        public List<string> Tags { get; internal set; } = new List<string>();
        /// <summary>
        /// Gets or sets the additional key-value properties associated with a project.
        /// These properties might be enriched by a transmitter plugin automatically
        /// (e.g. if a transmitter plugin uses reflection to scan through code for some extra info)
        /// </summary>
        /// <value>The properties.</value>
        public Dictionary<string, string> Properties { get; internal set; } = new Dictionary<string, string>();

        /// <summary>
        /// The download location
        /// </summary>
        public string DownloadLocation { get; set; }
        /// <summary>
        /// Gets or sets the components.
        /// </summary>
        /// <value>The components.</value>
        public List<ComponentManifestViewModel> Components { get; internal set; } = new List<ComponentManifestViewModel>();

        /// <summary>
        /// Gets the name of the assembly.
        /// </summary>
        /// <returns>System.String.</returns>
        public string GetOutputFileName()
        {
            if (!string.IsNullOrEmpty(this.AssemblyName))
            {
                return this.AssemblyName + "." + this.TargetExtension?.Trim('.').ToLower(CultureInfo.CurrentUICulture);
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
                return (this.ProjectName + this.AssemblyName + this.TargetExtension).Replace(".", "", StringComparison.InvariantCulture);
            }

            return "";
        }
    }
}