﻿using RepoCat.Transmission;
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
        /// Optional
        /// </summary>
        public bool ManifestCanOverrideRepositoryInfo { get; set; }

    }
}