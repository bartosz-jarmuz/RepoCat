using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.ApplicationInsights;
using RepoCat.Portal.RecurringJobs;
using RepoCat.Telemetry;
using RepoCat.Transmission.Client;

namespace RepoCat.Portal
{
    /// <summary>
    /// 
    /// </summary>
    public static class TelemetryExtensions
    {
           
        /// <summary>
        /// Before execution starts
        /// </summary>
        /// <param name="telemetryClient"></param>
        /// <param name="settings"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void TrackRecurringJobStarted(this TelemetryClient telemetryClient, RepositoryToScanSettings settings)
        {
            if (telemetryClient == null) throw new ArgumentNullException(nameof(telemetryClient));
            telemetryClient.TrackTrace(Telemetry.Names.RecurringJobStarted, GetProperties(settings));
        }

        /// <summary>
        /// Ater execution finished
        /// </summary>
        /// <param name="telemetryClient"></param>
        /// <param name="arguments"></param>
        /// <param name="result"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void TrackRecurringJobFinished(this TelemetryClient telemetryClient, TransmitterArguments arguments, RepositoryImportResult result)
        {
            if (telemetryClient == null) throw new ArgumentNullException(nameof(telemetryClient));
            var props = GetProperties(arguments);
            props.Add(PropertyKeys.SuccessCount, result.SuccessCount.ToString(CultureInfo.InvariantCulture));
            props.Add(PropertyKeys.FailedCount, result.FailedCount.ToString(CultureInfo.InvariantCulture));
            telemetryClient.TrackEvent(Telemetry.Names.RecurringJobFinished, props);
        }

        private static IDictionary<string, string> GetProperties(TransmitterArguments arguments)
        {
            return new Dictionary<string, string>()
            {
                {PropertyKeys.OrganizationName, arguments?.OrganizationName ?? "NULL"},
                {PropertyKeys.RepositoryName, arguments?.RepositoryName ?? "NULL"},
                {PropertyKeys.RepositoryPath, arguments?.CodeRootFolder ?? "NULL"},
                {PropertyKeys.RepositoryMode, arguments?.RepositoryMode.ToString()},
                {PropertyKeys.TransmissionMode, arguments?.TransmissionMode.ToString()},
            };
        }


        /// <summary>
        /// When added to schedule
        /// </summary>
        /// <param name="telemetryClient"></param>
        /// <param name="settings"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void TrackRecurringJobScheduled(this TelemetryClient telemetryClient, RepositoryToScanSettings settings)
        {
            if (telemetryClient == null) throw new ArgumentNullException(nameof(telemetryClient));
            telemetryClient.TrackEvent(Telemetry.Names.RecurringJobScheduled, GetProperties(settings));
        }


        private static Dictionary<string, string> GetProperties(RepositoryToScanSettings settings)
        {
            return new Dictionary<string, string>()
            {
                {PropertyKeys.OrganizationName, settings?.OrganizationName ?? "NULL"},
                {PropertyKeys.RepositoryName, settings?.RepositoryName ?? "NULL"},
                {PropertyKeys.RepositoryPath, settings?.RepositoryPath ?? "NULL"},
                {PropertyKeys.RepositoryMode, settings?.RepositoryMode.ToString()},
                {PropertyKeys.TransmissionMode, settings?.TransmissionMode.ToString() },
                {PropertyKeys.JobExecutionCron, settings?.JobExecutionCron ?? "NULL"},

            };
        }
    }
}