using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RepoCat.Portal.Models;
using RepoCat.Portal.Services;

namespace RepoCat.Portal.Controllers
{
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

        public async Task<ViewResult> Index(string repositoryName)
        {
            var model = new BrowseRepositoryViewModel()
            {
                RepoName = repositoryName
            };

            Tuple<string, List<ProjectManifest>> currentProjects = await this.service.GetCurrentProjects(repositoryName);
            List<ProjectManifestViewModel> manifests = this.mapper.Map<List<ProjectManifestViewModel>>(currentProjects.Item2);
            if (manifests.Any())
            {
                model.ProjectManifestViewModel = manifests;
                model.RepoStamp = currentProjects.Item1;
                var orderedTimes = manifests.OrderByDescending(x => x.AddedDateTime).ToList();
                model.ImportedDate = orderedTimes.First().AddedDateTime;
                model.ImportDuration = model.ImportedDate - orderedTimes.Last().AddedDateTime;
                model.NumberOfProjects = manifests.Count;
                model.NumberOfComponents = manifests.Sum(x=>x.Components.Count);
                model.NumberOfTags = manifests.Sum(prj => prj.Components.Sum(cmp=> cmp.Tags.Count));
            }

            return this.View(model);

        }

    }
}