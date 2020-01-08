using System;
using System.Diagnostics;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.ApplicationInsights;
using RepoCat.RepositoryManagement.Service;
using RepoCat.Transmission.Client;
using RepoCat.Transmission.Models;

namespace RepoCat.Portal.RecurringJobs
{
    /// <summary>
    /// 
    /// </summary>
    public class ScanRepositoryJob : IScanRepositoryJob
    {
        private readonly TelemetryClient telemetryClient;
        private readonly IMapper mapper;
        private readonly ITransmissionClient transmissionClient;

        /// <summary>
        /// The job of looking into specified repository for manifests and sending them to RepoCat
        /// </summary>
        /// <param name="telemetryClient"></param>
        /// <param name="mapper"></param>
        /// <param name="transmissionClient"></param>
        public ScanRepositoryJob(TelemetryClient telemetryClient, IMapper mapper, ITransmissionClient transmissionClient)
        {
            this.telemetryClient = telemetryClient;
            this.mapper = mapper;
            this.transmissionClient = transmissionClient;
        }

        /// <summary>
        /// Performs the job
        /// </summary>
        /// <param name="settings"></param>
        public async Task Run(RepositoryToScanSettings settings)
        {
            this.telemetryClient.TrackRecurringJobStarted(settings);

            if (settings == null) throw new ArgumentNullException(nameof(settings));

            var arguments = this.mapper.Map<TransmitterArguments>(settings);
            RepositoryImportResult result = await this.transmissionClient.Work(arguments).ConfigureAwait(false);
            this.telemetryClient.TrackRecurringJobFinished(arguments, result);
        }

     
    }
}
