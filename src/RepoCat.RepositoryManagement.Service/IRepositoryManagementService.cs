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
        Task<ManifestQueryResult> GetAllCurrentProjects(RepositoryQueryParameter repositoryQueryParameter);

        Task<ManifestQueryResult> GetCurrentProjects(IReadOnlyCollection<RepositoryQueryParameter> repoParams, string query, bool isRegex);

        Task<ManifestQueryResult> GetCurrentProjects(RepositoryQueryParameter repositoryQueryParameter, string query, bool isRegex);

        Task<IEnumerable<RepositoryInfo>> GetAllRepositories();

        /// <summary>
        /// Gets repositories grouped per organization name
        /// </summary>
        /// <returns></returns>
        Task<IReadOnlyCollection<RepositoryGrouping>> GetAllRepositoriesGrouped();


        Task<List<string>> GetStamps(RepositoryQueryParameter repositoryQueryParameter);
        Task<IEnumerable<CollectionSummary>> GetSummary();
    }
}