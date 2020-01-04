using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RepoCat.Persistence.Models;
using RepoCat.Persistence.Service;
using AutoMapper;
using Microsoft.ApplicationInsights;
using MongoDB.Driver;
using RepoCat.Telemetry;

namespace RepoCat.RepositoryManagement.Service
{
    public class RepositoryManagementService : IRepositoryManagementService
    {
        private readonly RepositoryDatabase database;
        private readonly IMapper mapper;
        private readonly TelemetryClient telemetryClient;

        public RepositoryManagementService(RepositoryDatabase database, IMapper mapper, TelemetryClient telemetryClient)
        {
            this.database = database;
            this.mapper = mapper;
            this.telemetryClient = telemetryClient;
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
                this.telemetryClient.TrackUpserted(repo);

                mappedProject.RepositoryId = repo.Id;
                ProjectInfo project;
                if (repo.RepositoryMode == RepositoryMode.Snapshot)
                {
                    project= await this.database.Create(mappedProject).ConfigureAwait(false);
                }
                else
                {
                    project = await this.database.Upsert(mappedProject).ConfigureAwait(false);
                }

                this.telemetryClient.TrackUpserted(project);
                return project;

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
