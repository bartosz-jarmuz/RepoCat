// -----------------------------------------------------------------------
//  <copyright file="IManifestQueryResultSorter.cs" company="bartosz.jarmuz@gmail.com">
//   Copyright (c) Bartosz Jarmuż. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using RepoCat.Persistence.Models;

namespace RepoCat.RepositoryManagement.Service
{
    public interface IManifestQueryResultSorter
    {
        IEnumerable<Project> Sort(IEnumerable<Project> projects, IEnumerable<string> searchTokens);
    }
}