using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RepoCat.Persistence.Service;
using ProjectInfo = RepoCat.Persistence.Models.ProjectInfo;

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
        public IActionResult Post(Transmitter.Models.ProjectInfo projectInfo)
        {
            ProjectInfo prjInfo = this.mapper.Map<ProjectInfo>(projectInfo);

            this.service.Create(prjInfo);

            return this.CreatedAtAction("Get", prjInfo.Id);
        }

        [HttpGet]
        public ProjectInfo Get(string id)
        {
            return this.service.Get(id);
        }

    }
}
