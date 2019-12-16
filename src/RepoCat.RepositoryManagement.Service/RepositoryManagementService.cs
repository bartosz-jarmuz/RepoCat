using System.Collections.Generic;
using System.Threading.Tasks;
using RepoCat.Persistence.Models;
using RepoCat.Persistence.Service;
using AutoMapper;
using MongoDB.Driver;

namespace RepoCat.RepositoryManagement.Service
{
    public class RepositoryManagementService : IRepositoryManagementService
    {
        private readonly RepositoryDatabase database;
        private readonly IMapper mapper;

        public RepositoryManagementService(RepositoryDatabase database, IMapper mapper)
        {
            this.database = database;
            this.mapper = mapper;

        }

        public async Task<ProjectInfo> Upsert(Transmission.Models.ProjectInfo projectInfo)
        {
            RepositoryInfo repo = await this.database.UpsertUpdate(projectInfo.OrganizationName, projectInfo.RepositoryName).ConfigureAwait(false);

            ProjectInfo result = this.mapper.Map<ProjectInfo>(projectInfo);
            result.RepositoryId = repo.Id;
            
            if (repo.RepositoryMode == RepositoryMode.Snapshot)
            {
                return await this.database.Create(result).ConfigureAwait(false);
            }
            else
            {
                return await this.database.Upsert(result).ConfigureAwait(false);
            }
        }

        public Task<ProjectInfo> GetById(string id)
        {
            return this.database.GetById(id);
        }

        public  Task<ManifestQueryResult> GetAllCurrentProjects(string organizationName, string repositoryName)
        {
            return this.database.GetAllCurrentProjects(organizationName, repositoryName);
        }

        public Task<ManifestQueryResult> GetAllCurrentProjects(RepositoryInfo repository)
        {
            return this.database.GetAllCurrentProjects(repository);
        }

        public async Task<IEnumerable<RepositoryInfo>> GetAllRepositories()
        {
            return (await this.database.GetAllRepositories().ConfigureAwait(false)).ToList();
        }


        public Task<ManifestQueryResult> GetCurrentProjects(string organizationName, string repositoryName,
            string query, bool isRegex)
        {
            return this.database.GetCurrentProjects(organizationName, repositoryName, query, isRegex);
        }
    }
}
