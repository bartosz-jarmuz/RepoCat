// -----------------------------------------------------------------------
//  <copyright file="IStatisticsService.cs" company="bartosz.jarmuz@gmail.com">
//   Copyright (c) Bartosz Jarmuż. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading.Tasks;

namespace RepoCat.RepositoryManagement.Service
{
    public interface IStatisticsService
    {
        Task<SearchStatistics> Get(RepositoryQueryParameter repositoryParameter);
        Task<SearchStatistics> Update(RepositoryQueryParameter repositoryParameter, IEnumerable<string> keywords);
        Task<IEnumerable<SearchKeywordData>> GetFlattened();
    }
}