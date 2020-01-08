using System;
using Cronos;
using Hangfire;
using Microsoft.ApplicationInsights;
using RepoCat.RepositoryManagement.Service;

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

                foreach (var repositorySetting in monitoringSettings.RepositorySettings)
                {
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
                    RecurringJob.RemoveIfExists(nameof(ScanRepositoryJob));
                    RecurringJob.AddOrUpdate<ScanRepositoryJob>(nameof(ScanRepositoryJob)
                        , job => job.Run(repositorySetting), expression);
                    telemetryClient.TrackRecurringJobScheduled(repositorySetting);
                }
            }
        }
    }
}