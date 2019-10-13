﻿using System.Collections.Generic;

namespace RepoCat.Transmission.Models
{
    /// <summary>
    /// Project info contains data about a project, understood as a class library or an executable.
    /// <para>Projects can contain zero or more components
    /// They can contain a set of plugins, tool or functionalities,
    /// however they can only be helper assemblies which do not contain any high-level components
    /// (in which case they are not interesting from the point of view of repository catalog).</para>
    /// <para>Project info is also different from a component manifest in that a project properties
    /// are automatically extracted from the project file (e.g. csproj),
    /// whereas component manifests are intended as a human-to-human communication</para>
    /// </summary>
    public class ProjectInfo
    {
        /// <summary>
        /// The name of the project (e.g. the name of the csproj file)
        /// </summary>
        public string ProjectName { get; set; }
        /// <summary>
        /// Gets or sets the project URI.
        /// </summary>
        /// <value>The project URI.</value>
        public string ProjectUri { get; set; }
        /// <summary>
        /// Gets or sets the name of the assembly generated by this project
        /// </summary>
        /// <value>The name of the assembly.</value>
        public string AssemblyName { get; set; }

        /// <summary>
        /// Gets or sets the type of the project output (class library, windows app etc)
        /// </summary>
        /// <value>The type of the output.</value>
        public string OutputType { get; set; }

        /// <summary>
        /// Gets or sets the target extension.
        /// </summary>
        /// <value>The target extension.</value>
        public string TargetExtension { get; set; }
        /// <summary>
        /// Gets or sets the name of the repository in which the project lives
        /// </summary>
        /// <value>The name of the repository.</value>
        public string RepositoryName { get; set; }
        /// <summary>
        /// A datetime or version stamp of the repository as of when the project info was read and transmitted
        /// </summary>
        public string RepositoryStamp { get; set; }

        /// <summary>
        /// Gets or sets the components manifests
        /// </summary>
        /// <value>The components.</value>
        public List<ComponentManifest> Components { get; set; } = new List<ComponentManifest>();

    }
}
