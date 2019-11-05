using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RepoCat.Persistence.Service;
using RepoCat.Portal.Areas.Catalog.Models;
using RepoCat.Portal.Utilities;
using RepoCat.Schemas;
using RepoCat.Persistence.Models;
using RepoCat.Transmission.Client;

namespace RepoCat.Portal.Areas.Catalog.Controllers
{
    /// <summary>
    /// Class RepositoryController.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.Controller" />
    [Area("Catalog")]
    [Route("Repository")]
    public class RepositoryController : Controller
    {
        private readonly ManifestsService service;
        private readonly IMapper mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryController"/> class.
        /// </summary>
        /// <param name="manifestsService">The manifests service.</param>
        /// <param name="mapper">The mapper.</param>
        public RepositoryController(ManifestsService manifestsService, IMapper mapper)
        {
            this.service = manifestsService;
            this.mapper = mapper;
        }

        /// <summary>
        /// Gets the repositories names
        /// </summary>
        /// <returns>Task&lt;IEnumerable&lt;System.String&gt;&gt;.</returns>
        public async Task<IEnumerable<string>> GetRepositoryNames()
        {
            return await this.service.GetRepositoryNames();
        }
        /// <summary>
        /// Indexes the specified repository name.
        /// </summary>
        /// <param name="repositoryName">Name of the repository.</param>
        /// <returns>Task&lt;ViewResult&gt;.</returns>
        [Route("{repositoryName}")]
        public async Task<ViewResult> Index(string repositoryName)
        {
            var model = new BrowseRepositoryViewModel()
            {
                RepositoryName = repositoryName
            };

            var result = await this.service.GetAllCurrentProjects(repositoryName);
            List<ProjectInfoViewModel> manifests = this.mapper.Map<List<ProjectInfoViewModel>>(result.ProjectInfos);
            if (manifests.Any())
            {
                model.ProjectManifestViewModels = manifests;
                model.RepositoryStamp = result.RepositoryStamp;
                var orderedTimes = manifests.OrderByDescending(x => x.AddedDateTime).ToList();
                model.ImportedDate = orderedTimes.First().AddedDateTime;
                model.ImportDuration = model.ImportedDate - orderedTimes.Last().AddedDateTime;
                model.NumberOfProjects = manifests.Count;
                model.NumberOfComponents = manifests.Sum(x=>x.Components.Count);
                model.NumberOfTags = manifests.Sum(prj => prj.Components.Sum(cmp=> cmp.Tags.Count));
            }

            return this.View(model);

        }

        /// <summary>
        /// Shows the add project view
        /// </summary>
        /// <returns>Task&lt;ViewResult&gt;.</returns>
        [HttpGet]
        public ViewResult AddProject()
        {
            return this.View(new AddProjectModel()
            {
                SampleManifestXml = SampleManifestXmlProvider.GetSampleProjectInfoSerialized()
                ,EmptyManifestXml = SampleManifestXmlProvider.GetEmptyProjectInfoSerialized()
            });
        }

        /// <summary>
        /// Adds the project.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <returns>Task&lt;IActionResult&gt;.</returns>
        [HttpPost]
#pragma warning disable 1998
        public async Task<IActionResult> AddProject([FromBody] AddProjectModel project)
#pragma warning restore 1998
        {
            if (!this.ModelState.IsValid)
            {
                this.TempData["error"] = "Incorrect input.";
                return Json(Url.Action("AddProject"));
            }

            try
            {

                var validator = new SchemaValidator();

                var errors = validator.ValidateComponentManifest(project.EmptyManifestXml, out XDocument _);

                if (errors.Count > 0)
                {
                    this.TempData["error"] = "Schema validation errors:\r\n" + string.Join("\r\n", errors);
                    return Json(Url.Action("AddProject"));
                }
                else
                {
                    var projectInfo =
                        ManifestDeserializer.DeserializeProjectInfo(XElement.Parse(project.EmptyManifestXml));

                    ProjectInfo mappedProjectInfo = this.mapper.Map<ProjectInfo>(projectInfo);

                    this.service.Create(mappedProjectInfo);

                    this.TempData["success"] = $"Added project to catalog. Internal ID: {mappedProjectInfo.Id}";

                    return Json(Url.Action("AddProject"));
                }
            }
            catch (Exception ex)
            {
                this.TempData["error"] = $"Error: {ex.Message}";
                return Json(Url.Action("AddProject"));
            }
        }
    }
}