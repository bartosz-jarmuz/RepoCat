// -----------------------------------------------------------------------
//  <copyright file="RepositoryController.cs" company="bartosz.jarmuz@gmail.com">
//   Copyright (c) Bartosz Jarmuż. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using AutoMapper;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;
using RepoCat.Persistence.Models;
using RepoCat.Portal.Areas.Catalog.Models;
using RepoCat.Portal.Controllers;
using RepoCat.Portal.Utilities;
using RepoCat.RepositoryManagement.Service;
using RepoCat.Schemas;
using RepoCat.Serialization;
using RepoCat.Utilities;
using SmartBreadcrumbs.Attributes;
using SmartBreadcrumbs.Nodes;
using DownloadStatistics = RepoCat.RepositoryManagement.Service.DownloadStatistics;
using ProjectInfo = RepoCat.Transmission.Models.ProjectInfo;
using RepositoryQueryParameter = RepoCat.RepositoryManagement.Service.RepositoryQueryParameter;

namespace RepoCat.Portal.Areas.Catalog.Controllers
{
    /// <summary>
    /// Class RepositoryController.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.Controller" />
    [Area("Catalog")]
    [Route("Repository")]
    [Breadcrumb("")]
    public class RepositoryController : Controller
    {
        private readonly IRepositoryManagementService repositoryService;
        private readonly IStatisticsService statisticsService;
        private readonly IMapper mapper;
        private readonly TelemetryClient telemetryClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryController"/> class.
        /// </summary>
        /// <param name="repositoryService">The repository management service.</param>
        /// <param name="statisticsService"></param>
        /// <param name="mapper"></param>
        /// <param name="telemetryClient"></param>
        public RepositoryController(IRepositoryManagementService repositoryService, IStatisticsService statisticsService, IMapper mapper, TelemetryClient telemetryClient)
        {
            this.repositoryService = repositoryService;
            this.mapper = mapper;
            this.telemetryClient = telemetryClient;
            this.statisticsService = statisticsService;
        }

        /// <summary>
        /// Gets the repositories names
        /// </summary>
        /// <returns>Task&lt;IEnumerable&lt;System.String&gt;&gt;.</returns>
        public async Task<IEnumerable<RepositoryInfo>> GetRepositoryNames()
        {
            return await this.repositoryService.GetAllRepositories().ConfigureAwait(false);
        }

        /// <summary>
        /// Indexes the specified repository name.
        /// </summary>
        /// <param name="organizationName"></param>
        /// <param name="repositoryName">Name of the repository.</param>
        /// <param name="filters">Optional string of filter definitions</param>
        /// <returns>Task&lt;ViewResult&gt;.</returns>
        [Route("{organizationName}/{repositoryName}")]
        public async Task<ViewResult> Index(string organizationName, string repositoryName, [FromQuery] Dictionary<string, List<string>> filters = null)
        {
            var model = new BrowseRepositoryViewModel()
            {
                RepositoryName = repositoryName,
            };

            this.telemetryClient.TrackViewRepository( organizationName, repositoryName);
            var queryParam = new RepositoryQueryParameter(organizationName, repositoryName);
            var repositoryInfo = (await this.repositoryService.GetRepositories(queryParam)).FirstOrDefault();
            if (repositoryInfo == null)
            {
                throw new InvalidOperationException("Repository not found");
            }

            var projectsTask = this.repositoryService.GetAllCurrentProjects(repositoryInfo);
            var stampsTask = this.repositoryService.GetStamps(repositoryInfo);
            var downloadStatsTask = this.statisticsService.GetDownloadStatistics(repositoryInfo);

            await Task.WhenAll(projectsTask, stampsTask, downloadStatsTask).ConfigureAwait(false);
            ManifestQueryResult result = projectsTask.Result;
            model.NumberOfStamps = stampsTask.Result.Count;
            model.RepositoryMode = result.Projects?.FirstOrDefault()?.RepositoryInfo?.RepositoryMode.ToString();
           List<ProjectInfoViewModel> manifests = this.mapper.Map<List<ProjectInfoViewModel>>(result.Projects);
            if (manifests.Any())
            {
                model.RepositoryStamp = StampSorter.GetNewestStamp(manifests.Select(x => x.RepositoryStamp).ToList());
                var orderedTimes = manifests.OrderByDescending(x => x.AddedDateTime).ToList();
                model.ImportedDate = orderedTimes.First().AddedDateTime;
                model.ImportDuration = model.ImportedDate - orderedTimes.Last().AddedDateTime;
                model.NumberOfProjects = manifests.Count;
                model.NumberOfAutogeneratedProjects = manifests.Count(x => x.Autogenerated);
                model.NumberOfComponents = manifests.Sum(x=>x.Components.Count);
                model.NumberOfTags = manifests.Sum(prj => prj.Components.Sum(cmp=> cmp.Tags.Count));
                foreach (ProjectInfoViewModel projectInfoViewModel in manifests)
                {
                    projectInfoViewModel.DownloadsCount =
                        downloadStatsTask.Result.ProjectDownloadData.FirstOrDefault(x =>
                            x.ProjectKey == projectInfoViewModel.ProjectUri)?.DownloadCount??0;
                }

            }
            ProjectsTableModel projectsTableModel = new ProjectsTableModel(manifests, false, false);

            model.ProjectsTable = projectsTableModel;
            model.ProjectsTable.Filters = filters;


            MvcBreadcrumbNode breadcrumb = PrepareIndexBreadcrumb(organizationName, repositoryName);

            this.ViewData["BreadcrumbNode"] = breadcrumb;
            return this.View(model);

        }

        private static MvcBreadcrumbNode PrepareIndexBreadcrumb(string organizationName, string repositoryName)
        {
            var breadCrumb = new MvcBreadcrumbNode(nameof(RepositoryController.Index), "Repository",
                organizationName, areaName: "Catalog")
            {
                RouteValues = new {organizationName = organizationName, repositoryName = repositoryName}
            };
            var breadCrumb2 = new MvcBreadcrumbNode(nameof(RepositoryController.Index), "Repository",
                repositoryName)
            {
                RouteValues = new {organizationName = organizationName, repositoryName = repositoryName},
                Parent = breadCrumb
            };
            var breadCrumb3 = new MvcBreadcrumbNode(nameof(RepositoryController.Index), "Repository",
                "Repository overview")
            {
                Parent = breadCrumb2
            };
            return breadCrumb3;
        }

        /// <summary>
        /// Shows the add project view
        /// </summary>
        /// <returns>Task&lt;ViewResult&gt;.</returns>
        [HttpGet]
        [Route("AddProject")]
        [Breadcrumb("Add Project", FromAction = "Index", FromController = typeof(HomeController))]
        public ViewResult AddProject()
        {
         
            return this.View(new AddProjectModel()
            {
                SampleManifestXml = SampleManifestXmlProvider.GetSampleProjectInfoSerialized(),
                EmptyManifestXml = SampleManifestXmlProvider.GetEmptyProjectInfoSerialized()
            });
        }

        /// <summary>
        /// Adds the project.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <returns>Task&lt;IActionResult&gt;.</returns>
        [HttpPost]
        [Route("AddProject")]
        [Breadcrumb("Add Project")]
        [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Whatever is wrong, just display the error")]
#pragma warning disable 1998
        public async Task<IActionResult> AddProject([FromBody] AddProjectModel project)
#pragma warning restore 1998
        {
            if (!this.ModelState.IsValid || project == null) 
            {
                this.TempData["error"] = "Incorrect input.";
                return this.Json(this.Url.Action("AddProject"));
            }

            try
            {

                var validator = new SchemaValidator();

                var errors = validator.ValidateManifest(project.EmptyManifestXml, out XDocument _);

                if (errors.Count > 0)
                {
                    this.TempData["error"] = "Schema validation errors:\r\n" + string.Join("\r\n", errors);
                    return this.Json(this.Url.Action("AddProject"));
                }
                else
                {
                    ProjectInfo projectInfo = ManifestDeserializer.DeserializeProjectInfo(XElement.Parse(project.EmptyManifestXml));

                    var upsertedProject = await this.repositoryService.Upsert(projectInfo).ConfigureAwait(false);

                    this.TempData["success"] = $"Added project to catalog. Internal ID: {upsertedProject.Id}";

                    return this.Json(this.Url.Action("AddProject"));
                }
            }
            catch (Exception ex)
            {
                this.TempData["error"] = $"Error: {ex.Message}";
                return this.Json(this.Url.Action("AddProject"));
            }
        }
    }
}