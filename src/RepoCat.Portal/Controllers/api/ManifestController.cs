using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using RepoCat.Models.ProjectInfo;
using RepoCat.Portal.Models;
using RepoCat.Portal.Services;

namespace RepoCat.Portal.Controllers.api
{
    [Route("api/manifest")]
    [ApiController]
    public class ManifestController : Controller
    {
        private readonly ManifestsService service;
        private readonly IMapper mapper;

        public ManifestController(ManifestsService manifestsService, IMapper mapper)
        {
            this.service = manifestsService;
            this.mapper = mapper;
        }

        [HttpPost]
        public IActionResult Post(ProjectInfo projectInfo)
        {
            ProjectManifest prjManifest = this.mapper.Map<ProjectManifest>(projectInfo);
            prjManifest.Components = ManifestDeserializer.LoadComponents(projectInfo.RepoCatManifest);

            this.service.Create(prjManifest);

            return this.CreatedAtAction("Get", prjManifest.Id);
        }

       

        [HttpGet]
        public ProjectManifest Get(string id)
        {
            return this.service.Get(id);
        }



        [HttpPost]
        [Route("Test")]
        public IActionResult Test()
        {
            var projectManifest = new ProjectManifest()
            {
                AssemblyName = "Some.dll",
                OutputType = "Class Library"
                ,
                Components = new List<ComponentManifest>()
                {
                    new ComponentManifest()
                    {
                        Description = "Some cool tool",
                        Tags = new List<string>() {"Utility", "Awesome", "Cool"},
                        Name = "Worker",

                    },
                    new ComponentManifest()
                    {
                        Description = "Not as cool tool",
                        Tags = new List<string>() {"Utility", "Awesome", "Cool"},
                        Name = "Worker",

                    },
                }
            };
            this.service.Create(projectManifest);

            return this.Ok();
        }

    }
}
