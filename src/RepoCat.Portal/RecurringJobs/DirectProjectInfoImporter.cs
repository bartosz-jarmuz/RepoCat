// -----------------------------------------------------------------------
//  <copyright file="DirectProjectInfoImporter.cs" company="SDL plc">
//   Copyright (c) SDL plc. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using RepoCat.RepositoryManagement.Service;
using RepoCat.Transmission;
using RepoCat.Transmission.Contracts;
using RepoCat.Transmission.Models;

namespace RepoCat.Portal.RecurringJobs
{
    /// <summary>
    /// 
    /// </summary>
    public class DirectProjectInfoImporter : ProjectInfoSenderBase
    {
        private readonly IRepositoryManagementService service;
        private readonly TelemetryClient telemetryClient;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="service"></param>
        /// <param name="telemetryClient"></param>
        public DirectProjectInfoImporter(IRepositoryManagementService service, TelemetryClient telemetryClient)
        {
            this.service = service;
            this.telemetryClient = telemetryClient;
        }

        /// <summary>
        /// Sends messages to telemetry
        /// </summary>
        protected override IProgress<ProjectImportProgressData> ProgressLog => new Progress<ProjectImportProgressData>(
            data =>
            {
                this.telemetryClient.TrackTrace(data.Message, new Dictionary<string, string>()
                {
                    {Telemetry.PropertyKeys.Verbosity, data.Verbosity.ToString()},
                    {Telemetry.PropertyKeys.Exception, data.Exception?.ToString()??""}
                });
            });


        ///<inheritdoc cref="IProjectInfoSender"/>
        public override async Task<ProjectImportResult> Send(ProjectInfo info)
        {
            var importResult = new ProjectImportResult(info);
            try
            {
                Persistence.Models.ProjectInfo importedProject = await this.service.Upsert(info).ConfigureAwait(false);
                this.telemetryClient.TrackTrace($"Inserted project with ID [{importedProject.Id}]");
                importResult.Success = true;
                importResult.Response = importedProject.Id.ToString();
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception ex)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                this.telemetryClient.TrackException(ex);
                importResult.Exception = ex;
            }

            return importResult;
        }


    }
}