using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using RepoCat.Persistence.Models;
using RepoCat.Persistence.Service;
using AutoMapper;
using Microsoft.ApplicationInsights;
using MongoDB.Driver;
using RepoCat.Telemetry;

namespace RepoCat.RepositoryManagement.Service
{
    public class StatisticsService : IStatisticsService
    {
        private readonly StatisticsDatabase database;
        private readonly IMapper mapper;

        public StatisticsService(StatisticsDatabase database, IMapper mapper)
        {
            this.database = database;
            this.mapper = mapper;
        }

        public async Task<SearchStatistics> Get(RepositoryQueryParameter repositoryParameter)
        {
            return await this.database.Get(
                this.mapper.Map<Persistence.Models.RepositoryQueryParameter>(repositoryParameter));
        }

        public async Task<IEnumerable<SearchStatistics>> Get()
        {
            return await this.database.Get();
        }

        public async Task<SearchStatistics> Update(RepositoryQueryParameter repositoryParameter, IEnumerable<string> keywords)
        {
            return await this.database.Update(
                this.mapper.Map<Persistence.Models.RepositoryQueryParameter>(repositoryParameter), keywords);
        }

    }
}
