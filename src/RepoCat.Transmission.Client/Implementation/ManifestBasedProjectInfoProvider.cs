// -----------------------------------------------------------------------
//  <copyright file="ManifestBasedProjectInfoProvider.cs" company="SDL plc">
//   Copyright (c) SDL plc. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using RepoCat.Transmission.Models;

namespace RepoCat.Transmission.Client
{
    public class ManifestBasedProjectInfoProvider : IProjectInfoProvider
    {
        public ProjectInfo GetInfo(string uri, RepositoryInfo repositoryInfo, string repoStamp)
        {
        }

        public IEnumerable<ProjectInfo> GetInfos(IEnumerable<string> uris, RepositoryInfo repositoryInfo, string repoStamp)
        {
        }
    }
}