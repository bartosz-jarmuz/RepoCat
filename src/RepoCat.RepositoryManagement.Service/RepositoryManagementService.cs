using System;
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
            ProjectInfo mappedProject;
            RepositoryInfo mappedRepo;
            try
            {
                mappedRepo = this.mapper.Map<RepositoryInfo>(projectInfo.RepositoryInfo);
                mappedProject = this.mapper.Map<ProjectInfo>(projectInfo);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Mapping exception", ex);
            }
          

            try
            {
                RepositoryInfo repo = await this.database.UpsertUpdate(mappedRepo).ConfigureAwait(false);
                mappedProject.RepositoryId = repo.Id;

                if (repo.RepositoryMode == RepositoryMode.Snapshot)
                {
                    return await this.database.Create(mappedProject).ConfigureAwait(false);
                }
                else
                {
                    return await this.database.Upsert(mappedProject).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Database write exception", ex);
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
