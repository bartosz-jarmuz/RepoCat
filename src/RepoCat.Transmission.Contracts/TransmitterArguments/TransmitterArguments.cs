﻿// -----------------------------------------------------------------------
//  <copyright file="TransmitterArguments.cs" company="bartosz.jarmuz@gmail.com">
//   Copyright (c) Bartosz Jarmuż. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using RepoCat.Transmission.Models;

namespace RepoCat.Transmission.Contracts
{
    /// <summary>
    /// The set of parameters for the worker.
    /// </summary>
    public class TransmitterArguments : ParameterSet
    {
        public TransmitterArguments() :base() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="TransmitterArguments"/> class.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public TransmitterArguments (string[] args) : base(args)
        {
         
        }

#pragma warning disable CA2227 // Collection properties should be read only
        public ICollection<string> ProjectPaths { get; set; }
#pragma warning restore CA2227 // Collection properties should be read only

        /// <summary>
        /// If the list of paths is too long (over 32k chars, which is max for cmd line argument) you can save the paths to a file and read from there
        /// </summary>
        public string ProjectPathsListInputFilePath { get; set; }

        /// <summary>
        /// Gets or sets the code root folder.
        /// </summary>
        /// <value>The code root folder.</value>
        public string CodeRootFolder { get; set; }
        /// <summary>
        /// Gets or sets the repository name.
        /// </summary>
        /// <value>The repo.</value>
        public string RepositoryName { get; set; }
        
        /// <summary>
        /// Gets or sets the organization name.
        /// </summary>
        /// <value>The repo.</value>
        public string OrganizationName { get; set; }

        /// <summary>
        /// Transmitter operation mode
        /// </summary>
        public TransmissionMode TransmissionMode { get; set; }

        /// <summary>
        /// Repository mode
        /// </summary>
        public RepositoryMode RepositoryMode { get; set; }

        /// <summary>
        /// Gets or sets the repo stamp (a datetime or version of the code base).
        /// </summary>
        /// <value>The repo stamp.</value>
        public string RepositoryStamp { get; set; }


        /// <summary>
        /// When the project info builder works based on software projects, some basic project info can be extracted from the project file.
        /// This means repository catalog can be filled in even with projects which don't contain an explicit manifest.
        /// Set this flag to true if you don't want projects without manifests to be sent to RepoCat
        /// </summary>
        public bool SkipProjectsWithoutManifest { get; set; }

        /// <summary>
        /// Exclude these paths from searching for manifests
        /// </summary>
        public string IgnoredPathsRegex { get; set; }

        /// <summary>
        /// Gets or sets the API base URI of the RepoCat instance that the transmitter is supposed to talk to.
        /// </summary>
        /// <value>The API base URI.</value>
        public Uri ApiBaseUri { get; set; }

        /// <summary>
        /// Set to true if it is possible for the manifest file to contain repository info which should override the setting in the transmitter arguments. <br/>
        /// The repository info in the manifest is ignored if the transmitter arguments contain the repository info. Set this flag to true, to change that behaviour.
        /// </summary>
        public bool ManifestCanOverrideRepositoryInfo { get; set; }

        /// <summary>
        /// A mapping of custom values to a ProjectInfo property name
        /// </summary>
        public Dictionary<string, string> PropertyMappings { get; set; } = new Dictionary<string, string>();

    }
}