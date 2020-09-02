// -----------------------------------------------------------------------
//  <copyright file="IRepositoryManagementService.cs" company="bartosz.jarmuz@gmail.com">
//   Copyright (c) Bartosz Jarmuż. All rights reserved.
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
        Task<ManifestQueryResult> GetAllCurrentProjects(RepositoryInfo repositoryInfo);

        Task<ManifestQueryResult> GetCurrentProjects(IReadOnlyCollection<RepositoryInfo> repositories, string query, bool isRegex);

        Task<ManifestQueryResult> GetCurrentProjects(RepositoryInfo repositoryInfo, string query, bool isRegex);

        Task<IEnumerable<RepositoryInfo>> GetAllRepositories();

        /// <summary>
        /// Gets repositories grouped per organization name
        /// </summary>
        /// <returns></returns>
        Task<IReadOnlyCollection<RepositoryGrouping>> GetAllRepositoriesGrouped();


        Task<List<string>> GetStamps(RepositoryInfo repositoryInfo);
        Task<IEnumerable<CollectionSummary>> GetSummary();
        Task<IEnumerable<RepositoryInfo>> GetRepositories(IReadOnlyCollection<RepositoryQueryParameter> repoParams);
        Task<IEnumerable<RepositoryInfo>> GetRepositories(RepositoryQueryParameter repoParam);
        Task<bool> DeleteRepository(RepositoryQueryParameter repoParam);
    }
}