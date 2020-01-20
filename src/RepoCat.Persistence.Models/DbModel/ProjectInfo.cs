﻿using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RepoCat.Persistence.Models
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
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        [BsonId,BsonRepresentation(BsonType.ObjectId), BsonIgnoreIfDefault]
        public ObjectId Id { get; set; }
        /// <summary>
        /// Gets or sets the date time of when the info was added to the database
        /// </summary>
        /// <value>The added date time.</value>
        public DateTime AddedDateTime { get; set; } = DateTime.UtcNow;
        /// <summary>
        /// The name of the project (e.g. the name of the csproj file)
        /// </summary>
        public string ProjectName { get; set; }

        /// <summary>
        /// Set to true if the project info was generated automatically, without a human-created manifest file
        /// </summary>
        public bool Autogenerated { get; set; }

        /// <summary>
        /// Who maintains the project at the moment
        /// </summary>
        public string Owner { get; set; }

        /// <summary>
        /// A description of problem
        /// </summary>
        public string ProjectDescription { get; set; }
        /// <summary>
        /// Gets or sets the project URI.
        /// </summary>
        /// <value>The project URI.</value>
        public string ProjectUri { get; set; }
        
        /// <summary>
        /// URI to general project documentation
        /// </summary>
        public string DocumentationUri { get; set; }

        /// <summary>
        /// Gets a location from which the file should be downloadable.
        /// The location should be accessible to user (if not the RepoCat webapp).
        /// Either URL or file path
        /// </summary>
        public string DownloadLocation { get; set; }
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
        /// Gets or sets the ID of the repository in which the project lives
        /// </summary>
        /// <value>The name of the repository.</value>
        public ObjectId RepositoryId { get; set; }
        /// <summary>
        /// A datetime or version stamp of the repository as of when the project info was read and transmitted
        /// </summary>
        public string RepositoryStamp { get; set; }

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
        /// Gets or sets the components manifests
        /// </summary>
        /// <value>The components.</value>
        public List<ComponentManifest> Components { get; internal set; } = new List<ComponentManifest>();
    }
}