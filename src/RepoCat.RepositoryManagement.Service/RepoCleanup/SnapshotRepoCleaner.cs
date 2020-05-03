// -----------------------------------------------------------------------
//  <copyright file="TelemetryExtensions.cs" company="bartosz.jarmuz@gmail.com">
//   Copyright (c) Bartosz Jarmuż. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using MongoDB.Driver;
using RepoCat.Persistence.Models;
using RepoCat.Persistence.Service;
using RepoCat.Telemetry;
using RepoCat.Utilities;

namespace RepoCat.RepositoryManagement.Service
{
    public class SnapshotRepoCleaner : ISnapshotRepoCleaner
    {
        private readonly RepositoryDatabase database;

        public SnapshotRepoCleaner(RepositoryDatabase database) => this.database = database;

        public async Task<SnapshotRepoCleanupResult> PerformCleanupAsync(SnapshotRepoCleanupSettings settings)
        {
            MongoDB.Driver.IAsyncCursor<RepositoryInfo> repos = await this.database.GetAllSnapshotRepositories();
            SnapshotRepoCleanupResult result = new SnapshotRepoCleanupResult();
            await repos.ForEachAsync(async repo =>
            {
                List<string> allStamps = await this.database.GetStamps(repo).ConfigureAwait(false);
                List<string> stampsToRemove = StampSorter.OrderStamps(allStamps).Skip(settings.NumberOfSnapshotsToKeep).ToList();
                foreach (string stamp in stampsToRemove)
                {
                    DeleteResult repoResults = await this.database.RemoveProjectsByStamp(repo, stamp);
                    if (result.RepositoryResults.ContainsKey(repo))
                    {
                        result.RepositoryResults[repo] += repoResults.DeletedCount;
                    }
                    else
                    {
                        result.RepositoryResults.Add(repo, repoResults.DeletedCount);
                    }
                }
            });
            return result;

        }
    }
}