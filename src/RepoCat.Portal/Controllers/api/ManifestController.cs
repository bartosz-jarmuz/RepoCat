using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RepoCat.Persistence.Models;
using RepoCat.Persistence.Service;
using RepoCat.RepositoryManagement.Service;
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
        private readonly IRepositoryManagementService service;

        /// <summary>
        /// Initializes a new instance of the <see cref="ManifestController"/> class.
        /// </summary>
        /// <param name="repositoryManagementService"></param>
        public ManifestController(IRepositoryManagementService repositoryManagementService)
        {
            this.service = repositoryManagementService;
        }

        /// <summary>
        /// Post the specified ProjectInfo to the database
        /// </summary>
        /// <param name="projectInfo">The project information.</param>
        /// <returns>IActionResult.</returns>
        [HttpPost]
        public async Task<IActionResult> Post(Transmission.Models.ProjectInfo projectInfo)
        {
            if (projectInfo == null)
            {
                return this.BadRequest("Project info is null");
            }

            try
            {
                ProjectInfo result = await this.service.Upsert(projectInfo).ConfigureAwait(false);
                return this.CreatedAtAction("Get", new { id = result.Id }, result);
            }
            catch (Exception)
            {
                return this.StatusCode(500);
            }
        }

        /// <summary>
        /// Gets the project info with specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>ProjectInfo.</returns>
        [HttpGet]
        public Task<ProjectInfo> Get(string id)
        {
            return this.service.GetById(id);
        }

    }
}
