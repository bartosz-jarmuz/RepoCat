// -----------------------------------------------------------------------
//  <copyright file="IRepositoryManagementService.cs" company="SDL plc">
//   Copyright (c) SDL plc. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading.Tasks;
using RepoCat.Persistence.Models;

namespace RepoCat.RepositoryManagement.Service
{
    public interface IRepositoryManagementService
    {
        Task<ProjectInfo> Upsert(Transmission.Models.ProjectInfo projectInfo);
        Task<ProjectInfo> GetById(string id);
        Task<ManifestQueryResult> GetAllCurrentProjects(string organizationName, string repositoryName);
        Task<IEnumerable<RepositoryInfo>> GetAllRepositories();
    }
}