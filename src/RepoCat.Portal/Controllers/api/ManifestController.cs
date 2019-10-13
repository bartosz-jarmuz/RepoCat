using System;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RepoCat.Persistence.Service;
using ProjectInfo = RepoCat.Persistence.Models.ProjectInfo;

namespace RepoCat.Portal.Controllers.api
{
    /// <summary>
    /// Handles requests related to project manifests
    /// </summary>
    /// <seealso cref="Controller" />
    [Route("api/manifest")]
    [ApiController]
    public class ManifestController : Controller
    {
        private readonly ManifestsService service;
        private readonly IMapper mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="ManifestController"/> class.
        /// </summary>
        /// <param name="manifestsService">The manifests service.</param>
        /// <param name="mapper">The mapper.</param>
        public ManifestController(ManifestsService manifestsService, IMapper mapper)
        {
            this.service = manifestsService;
            this.mapper = mapper;
        }

        /// <summary>
        /// Post the specified ProjectInfo to the database
        /// </summary>
        /// <param name="projectInfo">The project information.</param>
        /// <returns>IActionResult.</returns>
        [HttpPost]
        public IActionResult Post(Transmission.Models.ProjectInfo projectInfo)
        {
            ProjectInfo prjInfo;
            try
            {
                prjInfo = this.mapper.Map<ProjectInfo>(projectInfo);
            }
            catch (Exception)
            {
                return this.BadRequest("Error while mapping posted data.");
            }

            try
            {
                this.service.Create(prjInfo);
                return this.CreatedAtAction("Get", prjInfo.Id);
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        /// <summary>
        /// Gets the project info with specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>ProjectInfo.</returns>
        [HttpGet]
        public ProjectInfo Get(string id)
        {
            return this.service.Get(id);
        }

    }
}
