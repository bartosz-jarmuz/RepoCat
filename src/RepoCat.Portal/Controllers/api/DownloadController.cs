using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RepoCat.Persistence.Models;
using RepoCat.Persistence.Service;
using RepoCat.RepositoryManagement.Service;
using RepoCat.Telemetry;
using ProjectInfo = RepoCat.Persistence.Models.ProjectInfo;

namespace RepoCat.Portal.Controllers.api
{
    /// <summary>
    /// Handles requests related to project manifests
    /// </summary>
    /// <seealso cref="Controller" />
    [Route("api/download")]
    [ApiController]
    public class DownloadController : Controller
    {
        private readonly IRepositoryManagementService service;
        private readonly TelemetryClient telemetryClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="ManifestController"/> class.
        /// </summary>
        /// <param name="repositoryManagementService"></param>
        /// <param name="telemetryClient"></param>
        public DownloadController(IRepositoryManagementService repositoryManagementService, TelemetryClient telemetryClient)
        {
            this.service = repositoryManagementService;
            this.telemetryClient = telemetryClient;
        }


        /// <summary>
        /// Gets the project info with specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>ProjectInfo.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> Download(string id)
        {
            var project = await this.service.GetById(id).ConfigureAwait(false);
            if (project == null)
            {
                this.TempData["Error"] = $"Project [{id}] does not exist";
                return this.RedirectToAction("Error", "Home");
            }


            if (string.IsNullOrEmpty(project.DownloadLocation))
            {
                this.TempData["Error"] = $"Project [{id}] does not specify a download location";
                return this.RedirectToAction("Error", "Home");
            }



            if (IsLocalFile(project.DownloadLocation))
            {
                return this.GetLocalFileResult(project);
            }
            else
            {
                return this.GetRedirectResult(project);
            }
        }


        private static bool IsLocalFile(string projectDownloadLocation)
        {
            if (projectDownloadLocation == null) throw new ArgumentNullException(nameof(projectDownloadLocation));

            if (projectDownloadLocation.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
                || projectDownloadLocation.StartsWith("https://", StringComparison.OrdinalIgnoreCase)
                || projectDownloadLocation.StartsWith("www.", StringComparison.OrdinalIgnoreCase)
            )
            {
                return false;
            }

            return true;
        }

        private IActionResult GetRedirectResult(ProjectInfo project)
        {
            this.telemetryClient.TrackFileDownload(project, false);

            return this.Redirect(project.DownloadLocation);
        }


        private IActionResult GetLocalFileResult(ProjectInfo project)
        {
            var file = new FileInfo(project.DownloadLocation);
            if (!file.Exists)
            {
                this.TempData["Error"] = $"File {project.DownloadLocation} is not found or not accessible to RepoCat application. Try local access.";
                return this.RedirectToAction("Error", "Home");
            }
            this.Response.ContentLength = file.Length;

            this.telemetryClient.TrackFileDownload(project, true, file.Length);


#pragma warning disable CA2000 // Dispose objects before losing scope - the dispose is done by the File result. If you dispose it yourself, it will cause errors
            return this.File(System.IO.File.OpenRead(file.FullName), "application/octet-stream", file.Name);
#pragma warning restore CA2000 // Dispose objects before losing scope
        }
}
}
