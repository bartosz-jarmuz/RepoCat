using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using AutoMapper;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;
using RepoCat.Persistence.Service;
using RepoCat.Portal.Areas.Catalog.Models;
using RepoCat.Portal.Utilities;
using RepoCat.Schemas;
using RepoCat.Persistence.Models;
using RepoCat.Portal.Controllers;
using RepoCat.RepositoryManagement.Service;
using RepoCat.Serialization;
using RepoCat.Telemetry;
using RepoCat.Transmission;
using RepoCat.Utilities;
using SmartBreadcrumbs.Attributes;
using SmartBreadcrumbs.Nodes;
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
        private readonly IRepositoryManagementService service;
        private readonly IMapper mapper;
        private readonly TelemetryClient telemetryClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryController"/> class.
        /// </summary>
        /// <param name="service">The repository management service.</param>
        /// <param name="mapper"></param>
        /// <param name="telemetryClient"></param>
        public RepositoryController(IRepositoryManagementService service, IMapper mapper, TelemetryClient telemetryClient)
        {
            this.service = service;
            this.mapper = mapper;
            this.telemetryClient = telemetryClient;
        }

        /// <summary>
        /// Gets the repositories names
        /// </summary>
        /// <returns>Task&lt;IEnumerable&lt;System.String&gt;&gt;.</returns>
        public async Task<IEnumerable<RepositoryInfo>> GetRepositoryNames()
        {
            return await this.service.GetAllRepositories().ConfigureAwait(false);
        }

        /// <summary>
        /// Indexes the specified repository name.
        /// </summary>
        /// <param name="organizationName"></param>
        /// <param name="repositoryName">Name of the repository.</param>
        /// <returns>Task&lt;ViewResult&gt;.</returns>
        [Route("{organizationName}/{repositoryName}")]
        public async Task<ViewResult> Index(string organizationName, string repositoryName)
        {
            var model = new BrowseRepositoryViewModel()
            {
                RepositoryName = repositoryName
            };

            this.telemetryClient.TrackViewRepository( organizationName, repositoryName);
            var queryyParam = new RepositoryQueryParameter(organizationName, repositoryName);
            var projectsTask = this.service.GetAllCurrentProjects(queryyParam);
            var stampsTask = this.service.GetStamps(queryyParam);

            await Task.WhenAll(projectsTask, stampsTask).ConfigureAwait(false);
            ManifestQueryResult result = projectsTask.Result;
            model.NumberOfStamps = stampsTask.Result.Count;
            model.RepositoryMode = result?.Projects?.FirstOrDefault()?.RepositoryInfo?.RepositoryMode.ToString();

           List<ProjectInfoViewModel> manifests = this.mapper.Map<List<ProjectInfoViewModel>>(result.Projects);
            if (manifests.Any())
            {
                model.ProjectManifestViewModels = manifests;
                model.RepositoryStamp = StampSorter.GetNewestStamp(manifests.Select(x => x.RepositoryStamp).ToList());
                var orderedTimes = manifests.OrderByDescending(x => x.AddedDateTime).ToList();
                model.ImportedDate = orderedTimes.First().AddedDateTime;
                model.ImportDuration = model.ImportedDate - orderedTimes.Last().AddedDateTime;
                model.NumberOfProjects = manifests.Count;
                model.NumberOfComponents = manifests.Sum(x=>x.Components.Count);
                model.NumberOfTags = manifests.Sum(prj => prj.Components.Sum(cmp=> cmp.Tags.Count));
            }

            var breadCrumb = new MvcBreadcrumbNode(nameof(RepositoryController.Index), "Repository",
                organizationName, areaName: "Catalog")
            {
                
                RouteValues = new { organizationName = organizationName, repositoryName = repositoryName}
            };
            var breadCrumb2 = new MvcBreadcrumbNode(nameof(RepositoryController.Index), "Repository",
                repositoryName)
            {
                RouteValues = new { organizationName = organizationName, repositoryName = repositoryName },
                Parent = breadCrumb
                
            };
            var breadCrumb3 = new MvcBreadcrumbNode(nameof(RepositoryController.Index), "Repository",
                "Repository overview")
            {
                Parent = breadCrumb2
            };

            ViewData["BreadcrumbNode"] = breadCrumb3;
            return this.View(model);

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
            var empty = SampleManifestXmlProvider.GetEmptyProjectInfo();
            empty.RepositoryInfo.RepositoryName = "MISC";
            empty.RepositoryInfo.OrganizationName = "MISC";
            return this.View(new AddProjectModel()
            {
                SampleManifestXml = SampleManifestXmlProvider.GetSampleProjectInfoSerialized()
                ,EmptyManifestXml = ManifestSerializer.SerializeProjectInfo(empty).ToString()
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Whatever is wrong, just display the error")]
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
                    Transmission.Models.ProjectInfo projectInfo = ManifestDeserializer.DeserializeProjectInfo(XElement.Parse(project.EmptyManifestXml));

                    var upsertedProject = await this.service.Upsert(projectInfo).ConfigureAwait(false);

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