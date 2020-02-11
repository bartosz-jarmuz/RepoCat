// -----------------------------------------------------------------------
//  <copyright file="ManifestController.cs" company="bartosz.jarmuz@gmail.com">
//   Copyright (c) Bartosz Jarmuż. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Threading.Tasks;
using Hangfire;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;
using RepoCat.RepositoryManagement.Service;
using RepoCat.Transmission.Models;

namespace RepoCat.Portal.Controllers.api
{
    /// <summary>
    /// Handles requests related to project manifests
    /// </summary>
    /// <seealso cref="Controller" />
    [Route("api/manifest")]
    [ApiController]
    public class ManifestController : Controller
    {
        private readonly IRepositoryManagementService service;
        private readonly TelemetryClient telemetryClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="ManifestController"/> class.
        /// </summary>
        /// <param name="repositoryManagementService"></param>
        /// <param name="telemetryClient"></param>
        public ManifestController(IRepositoryManagementService repositoryManagementService, TelemetryClient telemetryClient)
        {
            this.service = repositoryManagementService;
            this.telemetryClient = telemetryClient;
        }

        /// <summary>
        /// Post the specified ProjectInfo to the database
        /// </summary>
        /// <param name="projectInfo">The project information.</param>
        /// <returns>IActionResult.</returns>
        [HttpPost]
        public IActionResult Post(ProjectInfo projectInfo)
        {
            if (projectInfo == null)
            {
                return this.BadRequest("Project info is null");
            }

            this.telemetryClient.TrackAdding(projectInfo);

            string jobId = BackgroundJob.Enqueue(() => this.service.Upsert(projectInfo));
            return this.Accepted(jobId);
          
        }

        /// <summary>
        /// Gets the project info with specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>ProjectInfo.</returns>
        [HttpGet]
        public Task<Persistence.Models.ProjectInfo> Get(string id)
        {
            return this.service.GetById(id);
        }

    }
}
