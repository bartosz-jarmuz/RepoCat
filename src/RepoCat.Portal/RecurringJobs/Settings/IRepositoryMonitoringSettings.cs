using System.Collections.Generic;

namespace RepoCat.Portal.RecurringJobs
{
    /// <summary>
    /// Collection of settings
    /// </summary>
    public interface IRepositoryMonitoringSettings
    {
        /// <summary>
        /// A collection of settings for each monitored repository
        /// </summary>
        List<RepositoryToScanSettings> RepositorySettings { get; set; }
    }
}