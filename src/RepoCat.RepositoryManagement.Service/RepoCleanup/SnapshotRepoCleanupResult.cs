using System.Collections.Generic;
using RepoCat.Persistence.Models;

namespace RepoCat.RepositoryManagement.Service
{
    public class SnapshotRepoCleanupResult
    {
        public Dictionary<RepositoryInfo, long> RepositoryResults { get; set; } = new Dictionary<RepositoryInfo, long>();
    }
}