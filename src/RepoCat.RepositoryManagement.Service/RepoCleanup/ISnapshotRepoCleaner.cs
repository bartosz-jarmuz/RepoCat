// -----------------------------------------------------------------------
//  <copyright file="ISnapshotRepoCleaner.cs" company="bartosz.jarmuz@gmail.com">
//   Copyright (c) Bartosz Jarmuż. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Threading.Tasks;

namespace RepoCat.RepositoryManagement.Service
{
    public interface ISnapshotRepoCleaner
    {
        Task<SnapshotRepoCleanupResult> PerformCleanupAsync(SnapshotRepoCleanupSettings settings);
    }
}