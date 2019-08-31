using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using MongoDB.Driver;
using RepoCat.Models.ProjectInfo;
using RepoCat.Portal.Models;
using RepoCat.Portal.Services;

namespace RepoCat.Portal.Controllers.api
{
    [Route("api/repository")]
    [ApiController]
    public class RepositoryController : Controller
    {
        private readonly ManifestsService service;

        public RepositoryController(ManifestsService manifestsService)
        {
            this.service = manifestsService;
        }

        public async Task<IEnumerable<string>> GetRepositories()
        {
            return await this.service.GetRepositories();
        }

    }
}