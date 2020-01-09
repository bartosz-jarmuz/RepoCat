// -----------------------------------------------------------------------
//  <copyright file="Project.cs" company="SDL plc">
//   Copyright (c) SDL plc. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace RepoCat.Persistence.Models
{
    /// <summary>
    /// Project component which contains a reference to it's repository and project details.<br/>
    /// It does not correspond to a specific database collection.<br/>
    /// It is assembled by performing a join during a query.
    /// </summary>
    public class Project
    {
        /// <summary>
        /// Details about the project
        /// </summary>
        public ProjectInfo ProjectInfo { get; set; }
        
        /// <summary>
        /// Repository reference
        /// </summary>
        public RepositoryInfo RepositoryInfo { get; set; }
    }
}