// -----------------------------------------------------------------------
//  <copyright file="ProjectDownloadData.cs" company="bartosz.jarmuz@gmail.com">
//   Copyright (c) Bartosz Jarmuż. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace RepoCat.RepositoryManagement.Service
{
    /// <summary>
    /// Statistics about download occurences of a certain project
    /// </summary>
    public class ProjectDownloadData
    {
        /// <summary>
        /// The identifier of a project which allows to determine which project is being downloaded <br/>
        /// Since the ProjectId field is only an internal one and the same project might be available under various IDs. <br/>
        /// The key should be e.g. a ProjectURI
        /// </summary>
        public string ProjectKey { get; set; }

        /// <summary>
        /// How many times a project was downloaded
        /// </summary>
        public int DownloadCount { get; set; }
    }
}