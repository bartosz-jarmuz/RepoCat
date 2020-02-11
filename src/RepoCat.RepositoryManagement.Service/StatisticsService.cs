// -----------------------------------------------------------------------
//  <copyright file="StatisticsService.cs" company="bartosz.jarmuz@gmail.com">
//   Copyright (c) Bartosz Jarmuż. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using RepoCat.Persistence.Service;

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
            Persistence.Models.SearchStatistics result = await this.database.Get(
                this.mapper.Map<Persistence.Models.RepositoryQueryParameter>(repositoryParameter));
            return this.mapper.Map<SearchStatistics>(result);
        }

        public async Task<IEnumerable<SearchKeywordData>> GetFlattened()
        {
            var result = await this.database.Get();
            return this.FlattenStats(result);
        }

        private List<SearchKeywordData> FlattenStats(IEnumerable<Persistence.Models.SearchStatistics> result)
        {
            var list = new List<SearchKeywordData>();
            foreach (Persistence.Models.SearchStatistics searchStatisticse in result)
            {
                foreach (var data in searchStatisticse.SearchKeywordData)
                {
                    var existing = list.FirstOrDefault(x => string.Equals(data.Keyword, x.Keyword));
                    if (existing == null)
                    {
                        list.Add(
                            new SearchKeywordData()
                            {
                                Keyword = data.Keyword,
                                SearchCount = data.SearchCount
                            }
                        );
                    }
                    else
                    {
                        existing.SearchCount += data.SearchCount;
                    }
                }
            }

            return list;
        }

        public async Task<SearchStatistics> Update(RepositoryQueryParameter repositoryParameter, IEnumerable<string> keywords)
        {
            var result = await this.database.Update(
                this.mapper.Map<Persistence.Models.RepositoryQueryParameter>(repositoryParameter), keywords);
            return this.mapper.Map<SearchStatistics>(result);
        }

    }
}
