using System;
using System.Collections.Generic;
using RepoCat.Transmission;
using RepoCat.Transmission.Contracts;
using RepoCat.Transmission.Models;

namespace RepoCat.Portal.RecurringJobs
{

    /// <summary>
    /// Settings of the repo to scan
    /// </summary>
    public class RepositoryToScanSettings
    {
        /// <summary>
        /// If the list of paths is too long (over 32k chars, which is max for cmd line argument) you can save the paths to a file and read from there
        /// </summary>
        public string JobName { get; set; } = Guid.NewGuid().ToString().Remove(5);
        /// <summary>
        /// If the list of paths is too long (over 32k chars, which is max for cmd line argument) you can save the paths to a file and read from there
        /// </summary>
        public string ProjectPathsListInputFilePath { get; set; }


        /// <summary>
        /// Path to repository
        /// </summary>
        public string RepositoryPath { get; set; }

        /// <summary>
        /// Repository name
        /// </summary>
        public string RepositoryName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string OrganizationName { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public RepositoryMode RepositoryMode { get; set; }
        
        /// <summary>
        /// Defines what project info builder will be used (e.g. manifest based, .NET project based etc)
        /// </summary>
        public TransmissionMode TransmissionMode { get; set; }

        /// <summary>
        /// How often and when should it be executed
        /// </summary>
        public string JobExecutionCron { get; set; }
        
        /// <summary>
        /// Optionally, what should be excuded
        /// </summary>
        public string IgnoredPathsRegex { get; set; }

        /// <summary>
        /// When the project info builder works based on software projects, some basic project info can be extracted from the project file.
        /// This means repository catalog can be filled in even with projects which don't contain an explicit manifest.
        /// Set this flag to true if you don't want projects without manifests to be sent to RepoCat
        /// </summary>
        public bool SkipProjectsWithoutManifest { get; set; }

        /// <summary>
        /// Optional
        /// </summary>
        public bool ManifestCanOverrideRepositoryInfo { get; set; }

        /// <summary>
        /// A mapping of custom values to a ProjectInfo property name
        /// </summary>
        public Dictionary<string, string> PropertyMappings { get; set; } = new Dictionary<string, string>();

    }
}