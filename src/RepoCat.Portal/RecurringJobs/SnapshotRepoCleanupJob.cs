// -----------------------------------------------------------------------
//  <copyright file="ScanRepositoryJob.cs" company="bartosz.jarmuz@gmail.com">
//   Copyright (c) Bartosz Jarmuż. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.ApplicationInsights;
using RepoCat.RepositoryManagement.Service;
using RepoCat.Transmission.Contracts;

namespace RepoCat.Portal.RecurringJobs
{
    /// <summary>
    /// 
    /// </summary>
    public class SnapshotRepoCleanupJob
    {
        private readonly TelemetryClient telemetryClient;
        private readonly IMapper mapper;
        private readonly ISnapshotRepoCleaner cleaner;

        /// <summary>
        /// The job of looking into specified repository for manifests and sending them to RepoCat
        /// </summary>
        /// <param name="telemetryClient"></param>
        /// <param name="mapper"></param>
        /// <param name="cleaner"></param>
        public SnapshotRepoCleanupJob(TelemetryClient telemetryClient, IMapper mapper, ISnapshotRepoCleaner cleaner)
        {
            this.telemetryClient = telemetryClient;
            this.mapper = mapper;
            this.cleaner = cleaner;
        }

        /// <summary>
        /// Performs the job
        /// </summary>
        public async Task Run()
        {
            var sw = Stopwatch.StartNew();

            var result = await this.cleaner.PerformCleanupAsync(new SnapshotRepoCleanupSettings()
            {
                NumberOfSnapshotsToKeep = 2
            });
            sw.Stop();
            
            this.telemetryClient.TrackCleanupJob(result, sw.ElapsedMilliseconds);
        }

     
    }
}
