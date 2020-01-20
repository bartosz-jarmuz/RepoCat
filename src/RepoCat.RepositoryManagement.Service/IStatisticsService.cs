// -----------------------------------------------------------------------
//  <copyright file="IStatisticsService.cs" company="SDL plc">
//   Copyright (c) SDL plc. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading.Tasks;
using RepoCat.Persistence.Models;

namespace RepoCat.RepositoryManagement.Service
{
    public interface IStatisticsService
    {
        Task<SearchStatistics> Get(RepositoryQueryParameter repositoryParameter);
        Task<SearchStatistics> Update(RepositoryQueryParameter repositoryParameter, IEnumerable<string> keywords);
        Task<IEnumerable<SearchStatistics>> Get();
    }
}