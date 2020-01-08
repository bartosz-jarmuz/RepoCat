using RepoCat.Transmission.Client;
using RepoCat.Transmission.Models;

namespace RepoCat.Portal.RecurringJobs
{
    /// <inheritdoc cref="IRepositoryToScanSettings"/>
    public class RepositoryToScanSettings 
    {
        /// <inheritdoc cref="IRepositoryToScanSettings"/>
        public string RepositoryPath { get; set; }
        /// <inheritdoc cref="IRepositoryToScanSettings"/>
        public string RepositoryName { get; set; }
        /// <inheritdoc cref="IRepositoryToScanSettings"/>
        public string OrganizationName { get; set; }
        /// <inheritdoc cref="IRepositoryToScanSettings"/>
        public RepositoryMode RepositoryMode { get; set; }
        /// <inheritdoc cref="IRepositoryToScanSettings"/>
        public TransmissionMode TransmissionMode { get; set; }
        /// <inheritdoc cref="IRepositoryToScanSettings"/>
        public string JobExecutionCron { get; set; }
        public string IgnoredPathsRegex { get; set; }
        public bool ManifestCanOverrideRepositoryInfo { get; set; }

    }
}