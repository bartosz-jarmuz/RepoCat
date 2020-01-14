using System.Collections.Generic;

namespace RepoCat.Portal.RecurringJobs
{
    /// <inheritdoc cref="IRepositoryMonitoringSettings"/>
    public class RepositoryMonitoringSettings : IRepositoryMonitoringSettings
    {
        /// <inheritdoc cref="IRepositoryMonitoringSettings"/>
        #pragma warning disable CA2227
        public List<RepositoryToScanSettings> RepositorySettings { get; set; } = new List<RepositoryToScanSettings>();
#pragma warning restore CA2227
    }
}