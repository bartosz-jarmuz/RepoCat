// -----------------------------------------------------------------------
//  <copyright file="SortedManifestQueryResult.cs" company="bartosz.jarmuz@gmail.com">
//   Copyright (c) Bartosz Jarmuż. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using RepoCat.Persistence.Models;

namespace RepoCat.RepositoryManagement.Service
{
    /// <summary>
    /// Results with projects sorted by how well they match the search criteria
    /// </summary>
    public class SortedManifestQueryResult  : ManifestQueryResult
    {
        public SortedManifestQueryResult(IEnumerable<RepositoryQueryParameter> repoParams, IEnumerable<Project> projects, TimeSpan elapsed, string queryString, bool isRegex) : base(repoParams, projects, elapsed, queryString, isRegex)
        {
        }

        public IEnumerable<KeyValuePair<Project, int>> Sorted { get; } = new List<KeyValuePair<Project, int>>();

    }
}