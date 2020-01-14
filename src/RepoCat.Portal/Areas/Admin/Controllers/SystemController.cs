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
using RepoCat.Portal.Areas.Admin.Models;
using RepoCat.Portal.Controllers;
using RepoCat.RepositoryManagement.Service;
using RepoCat.Telemetry;
using RepoCat.Transmission;
using RepoCat.Utilities;
using SmartBreadcrumbs.Attributes;
using SmartBreadcrumbs.Nodes;
using CollectionSummary = RepoCat.Persistence.Models.CollectionSummary;
using RepositoryQueryParameter = RepoCat.RepositoryManagement.Service.RepositoryQueryParameter;

namespace RepoCat.Portal.Areas.Admin.Controllers
{
    /// <summary>
    /// Class RepositoryController.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.Controller" />
    [Area("Admin")]
    [Route("System")]
    public class SystemController : Controller
    {
        private readonly IRepositoryManagementService service;
        private readonly IMapper mapper;
        private readonly TelemetryClient telemetryClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="SystemController"/> class.
        /// </summary>
        /// <param name="service">The repository management service.</param>
        /// <param name="mapper"></param>
        /// <param name="telemetryClient"></param>
        public SystemController(IRepositoryManagementService service, IMapper mapper, TelemetryClient telemetryClient)
        {
            this.service = service;
            this.mapper = mapper;
            this.telemetryClient = telemetryClient;
        }

        /// <summary>
        /// Default view
        /// </summary>
        /// <returns>Task&lt;ViewResult&gt;.</returns>
        [Breadcrumb("Database Overview")]
        public async Task<ViewResult> Index()
        {
            var model = new DatabaseOverviewViewModel();
            IEnumerable<CollectionSummary> summaries = await this.service.GetSummary().ConfigureAwait(false);
            IEnumerable<Models.CollectionSummary> mapped = this.mapper.Map<IEnumerable<Models.CollectionSummary>>(summaries);
            model.Collections = new List<RepoCat.Portal.Areas.Admin.Models.CollectionSummary>(mapped);
           
            return this.View(model);

        }

    }
}