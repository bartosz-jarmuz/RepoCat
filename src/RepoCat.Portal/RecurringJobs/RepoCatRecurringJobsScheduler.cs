﻿// -----------------------------------------------------------------------
//  <copyright file="RepoCatRecurringJobsScheduler.cs" company="bartosz.jarmuz@gmail.com">
//   Copyright (c) Bartosz Jarmuż. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using Cronos;
using Hangfire;
using Microsoft.ApplicationInsights;

namespace RepoCat.Portal.RecurringJobs
{
    /// <summary>
    /// 
    /// </summary>
    public static class RepoCatRecurringJobsScheduler
    {
        /// <summary>
        /// 
        /// </summary>
        public static void ScheduleRecurringJobs(IRepositoryMonitoringSettings monitoringSettings,
            TelemetryClient telemetryClient)
        {
            if (monitoringSettings != null)
            {
                if (telemetryClient == null) throw new ArgumentNullException(nameof(telemetryClient));

                for (int index = 0; index < monitoringSettings.RepositorySettings.Count; index++)
                {
                    var repositorySetting = monitoringSettings.RepositorySettings[index];
                    var expression = repositorySetting.JobExecutionCron ?? "0 * * * *";
                    try
                    {
                        var _ = CronExpression.Parse(expression);
                    }
#pragma warning disable CA1031 // Do not catch general exception types
                    catch (Exception ex)
#pragma warning restore CA1031 // Do not catch general exception types
                    {
                        telemetryClient.TrackException(new InvalidOperationException(
                            $"Error while trying to convert expression {expression} as CRON", ex));
                        continue;
                    }

                    RecurringJob.RemoveIfExists(nameof(ScanRepositoryJob)+ repositorySetting.JobName);
                    RecurringJob.AddOrUpdate<ScanRepositoryJob>(nameof(ScanRepositoryJob) + repositorySetting.JobName
                        , job => job.Run(repositorySetting), expression);
                    telemetryClient.TrackRecurringJobScheduled(repositorySetting);
                }
            }

            RecurringJob.AddOrUpdate<SnapshotRepoCleanupJob>(nameof(SnapshotRepoCleanupJob), job=> job.Run(), "0 * * * *");
        }
    }
}