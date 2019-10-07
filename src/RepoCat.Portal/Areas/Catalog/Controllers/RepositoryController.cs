using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RepoCat.Persistence.Service;
using RepoCat.Portal.Areas.Catalog.Models;
using RepoCat.Portal.Models;
using RepoCat.Portal.Services;

namespace RepoCat.Portal.Controllers
{
    [Area("Catalog")]
    [Route("Repository")]
    public class RepositoryController : Controller
    {
        private readonly ManifestsService service;
        private readonly IMapper mapper;

        public RepositoryController(ManifestsService manifestsService, IMapper mapper)
        {
            this.service = manifestsService;
            this.mapper = mapper;
        }

        public async Task<IEnumerable<string>> GetRepositories()
        {
            return await this.service.GetRepositories();
        }

        [Route("{repositoryName}")]
        public async Task<ViewResult> Index(string repositoryName)
        {
            var model = new BrowseRepositoryViewModel()
            {
                RepoName = repositoryName
            };

            var result = await this.service.GetAllCurrentProjects(repositoryName);
            List<ProjectManifestViewModel> manifests = this.mapper.Map<List<ProjectManifestViewModel>>(result.Manifests);
            if (manifests.Any())
            {
                model.ProjectManifestViewModels = manifests;
                model.RepoStamp = result.RepoStamp;
                var orderedTimes = manifests.OrderByDescending(x => x.AddedDateTime).ToList();
                model.ImportedDate = orderedTimes.First().AddedDateTime;
                model.ImportDuration = model.ImportedDate - orderedTimes.Last().AddedDateTime;
                model.NumberOfProjects = manifests.Count;
                model.NumberOfComponents = manifests.Sum(x=>x.Components.Count);
                model.NumberOfTags = manifests.Sum(prj => prj.Components.Sum(cmp=> cmp.Tags.Count));
            }

            return this.View(model);

        }

        [HttpGet]
        public async Task<ViewResult> AddProject()
        {
            await Task.Delay(0);
            return this.View(new AddProjectModel()
            {
                ManifestXml = "<root>hello</root>"
            });
        }

        [HttpPost]
        public async Task<IActionResult> AddProject(AddProjectModel project)
        {
            if (!ModelState.IsValid)
            {
                this.TempData["error"] = "Incorrect input.";
                return this.View(project);
            }

            this.TempData["success"] = "Added project to catalog";
            return this.View(new AddProjectModel());


        }
    }
}