// -----------------------------------------------------------------------
//  <copyright file="DownloadController.cs" company="bartosz.jarmuz@gmail.com">
//   Copyright (c) Bartosz Jarmuż. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;
using RepoCat.Persistence.Models;
using RepoCat.RepositoryManagement.Service;

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
        /// <param name="componentName"></param>
        /// <param name="propertyKey"></param>
        /// <returns>ProjectInfo.</returns>
        [HttpGet("{id}/{componentName}/{propertyKey}")]
        public async Task<IActionResult> Download(string id, string componentName, string propertyKey)
        {
            var project = await this.service.GetById(id).ConfigureAwait(false);
            if (project == null)
            {
                this.TempData["Error"] = $"Project [{id}] does not exist";
                return this.RedirectToAction("Error", "Home");
            }

            if (!string.IsNullOrEmpty(componentName))
            {
                var component = project.Components.FirstOrDefault(x => x.Name == componentName);
                if (component == null)
                {
                    this.TempData["Error"] = $"Component [{componentName}] does not exist";
                    return this.RedirectToAction("Error", "Home");
                }

                if (propertyKey == nameof(ComponentManifest.DocumentationUri))
                {
                    return this.GetLocalFileResult(project, component.DocumentationUri);
                }

                Property property = component.Properties.FirstOrDefault(x => x.Key == propertyKey);
                if (property?.Value == null)
                {
                    this.TempData["Error"] = $"Property [{propertyKey}] does not exist";
                    return this.RedirectToAction("Error", "Home");
                }
                return this.GetLocalFileResult(project, property.Value?.ToString());
            }
            else
            {
                Property property = project.Properties.FirstOrDefault(x => x.Key == propertyKey);
                if (property?.Value == null)
                {
                    this.TempData["Error"] = $"Project property [{propertyKey}] does not exist";
                    return this.RedirectToAction("Error", "Home");
                }
                return this.GetLocalFileResult(project, property.Value?.ToString());
            }
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
                return this.GetLocalFileResult(project, project.DownloadLocation);
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


        private IActionResult GetLocalFileResult(ProjectInfo project, string filePath)
        {
            var file = new FileInfo(filePath);
            if (!file.Exists)
            {
                this.TempData["Error"] = $"File {filePath} is not found or not accessible to RepoCat application. Try local access.";
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
