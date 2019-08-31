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
                model.RepoStamp = currentProjects.Item1;
                model.ProjectManifestViewModel = manifests;
                model.NumberOfProjects = manifests.Count;
                model.ImportedDate = manifests.OrderByDescending(x => x.AddedDateTime).First().AddedDateTime;

            }

            return this.View(model);

        }

    }
}