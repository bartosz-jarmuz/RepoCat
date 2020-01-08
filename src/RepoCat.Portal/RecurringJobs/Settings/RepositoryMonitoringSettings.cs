using System.Collections.Generic;

namespace RepoCat.Portal.RecurringJobs
{
    /// <inheritdoc cref="IRepositoryMonitoringSettings"/>
    public class RepositoryMonitoringSettings : IRepositoryMonitoringSettings
    {
        /// <inheritdoc cref="IRepositoryMonitoringSettings"/>

        public List<RepositoryToScanSettings> RepositorySettings { get; set; } = new List<RepositoryToScanSettings>();
    }
}