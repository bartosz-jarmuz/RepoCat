// -----------------------------------------------------------------------
//  <copyright file="Project.cs" company="bartosz.jarmuz@gmail.com">
//   Copyright (c) Bartosz Jarmuż. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace RepoCat.Persistence.Models
{
    /// <summary>
    ///     Project component which contains a reference to it's repository and project details.<br />
    ///     It does not correspond to a specific database collection.<br />
    ///     It is assembled by performing a join during a query.
    /// </summary>
    public class Project
    {
        /// <summary>
        /// How relevant should be the result for a given query
        /// </summary>
        public decimal SearchAccuracyScore { get; set; }

        /// <summary>
        ///     Details about the project
        /// </summary>
        public ProjectInfo ProjectInfo { get; set; }

        /// <summary>
        ///     Repository reference
        /// </summary>
        public RepositoryInfo RepositoryInfo { get; set; }
    }
}