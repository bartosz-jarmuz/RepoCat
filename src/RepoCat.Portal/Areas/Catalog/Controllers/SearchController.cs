﻿// -----------------------------------------------------------------------
//  <copyright file="SearchController.cs" company="bartosz.jarmuz@gmail.com">
//   Copyright (c) Bartosz Jarmuż. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Hangfire;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RepoCat.Persistence.Models;
using RepoCat.Portal.Areas.Catalog.Models;
using RepoCat.RepositoryManagement.Service;
using RepoCat.Utilities;
using SmartBreadcrumbs.Attributes;
using RepositoryQueryParameter = RepoCat.RepositoryManagement.Service.RepositoryQueryParameter;

namespace RepoCat.Portal.Areas.Catalog.Controllers
{
    /// <summary>
    /// Handles search requests
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.Controller" />
    [Area("Catalog")]
    public class SearchController : Controller
    {
        private readonly IMapper mapper;
        private readonly TelemetryClient telemetryClient;
        private readonly IRepositoryManagementService repositoryService;
        private readonly IStatisticsService statisticsService;


        /// <summary>
        /// Initializes a new instance of the <see cref="SearchController"/> class.
        /// </summary>
        /// <param name="mapper">The mapper.</param>
        /// <param name="telemetryClient"></param>
        /// <param name="repositoryService"></param>
        /// <param name="statisticsService"></param>
        public SearchController(IMapper mapper, TelemetryClient telemetryClient, IRepositoryManagementService repositoryService, IStatisticsService statisticsService)
        {
            this.mapper = mapper;
            this.telemetryClient = telemetryClient;
            this.repositoryService = repositoryService;
            this.statisticsService = statisticsService;
        }

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns>Task&lt;IActionResult&gt;.</returns>
        [Breadcrumb("Search")]
        public async Task<IActionResult> Index()
        {
            SearchIndexViewModel model = new SearchIndexViewModel();
            var searchStatsTask = this.statisticsService.GetFlattened();
            model.Repositories = await this.GetRepositoriesSelectList().ConfigureAwait(false);
            await searchStatsTask.ConfigureAwait(false);
            model.TopSearchedTags = searchStatsTask.Result.ToList();
            return this.View(model);
        }

        

        private async Task<List<SelectListItem>> GetRepositoriesSelectList()
        {
            IReadOnlyCollection<RepositoryGrouping> groups = await this.repositoryService.GetAllRepositoriesGrouped().ConfigureAwait(false);

            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem() { Text = "[All organizations, all repositories]", Value = $"*:*"});

            foreach (RepositoryGrouping repoGroup in groups)
            {
                SelectListGroup group = new SelectListGroup() { Name = repoGroup.OrganizationName };
                items.Add(new SelectListItem() { Text = "[All repositories]", Value = $"{repoGroup.OrganizationName}:*", Group = group });

                foreach (RepositoryInfo repo in repoGroup.Repositories)
                {
                    SelectListItem item = new SelectListItem() { Text = repo.RepositoryName, Value = $"{repo.OrganizationName}:{repo.RepositoryName}", Group = group };
                    items.Add(item);
                }

            }
            return items;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="org"></param>
        /// <param name="repo"></param>
        /// <param name="query"></param>
        /// <param name="isRegex"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Search")]
        public async Task<IActionResult> Search(string[] org, string[] repo, string query, bool isRegex)
        {
            if (org == null) throw new ArgumentNullException(nameof(org));
            if (repo == null) throw new ArgumentNullException(nameof(repo));
            if (org.Length != repo.Length)
            {
                this.TempData["Error"] = $"Number of org parameters does not match the number of repo parameters. Orgs: {string.Join(", ", org)}. Repos: {string.Join(", ", repo)}";
                return this.RedirectToAction("Error", "Home");
            }

            IReadOnlyCollection<RepositoryQueryParameter> parameters = RepositoryQueryParameter.ConvertFromArrays(org, repo);
            BackgroundJob.Enqueue(() => this.UpdateSearchStatistics(parameters, query));

            ManifestQueryResultViewModel queryResultViewModel = await this.GetQueryResultViewModel(parameters, query, isRegex).ConfigureAwait(false);
            this.telemetryClient.TrackSearch(parameters, query, isRegex, queryResultViewModel.ProjectsTable.Projects.Count, queryResultViewModel.Elapsed);
            return this.PartialView("_SearchResultPartial", queryResultViewModel);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task UpdateSearchStatistics(IReadOnlyCollection<RepositoryQueryParameter> parameters, string query)
        {
            var keywords = QueryStringTokenizer.GetTokens(query);
            foreach (RepositoryQueryParameter repositoryQueryParameter in parameters)
            {
                await this.statisticsService.Update(repositoryQueryParameter, keywords);
            }
        }



        /// <summary>
        /// Gets the search result page (for URL sharing).
        /// </summary>
        /// <param name="org">Names of the organizations which holds the repository</param>
        /// <param name="repo">Names of the repositories.</param>
        /// <param name="query">The query.</param>
        /// <param name="isRegex">if set to <c>true</c> [is regex].</param>
        /// <returns>Task&lt;IActionResult&gt;.</returns>
        [HttpGet]
        [Route("{controller}/Result")]
        public async Task<IActionResult> GetSearchResultPage(string[] org, string[] repo, string query, bool isRegex)
        {
            if (org == null) throw new ArgumentNullException(nameof(org));
            if (repo == null) throw new ArgumentNullException(nameof(repo));
            if (org.Length != repo.Length)
            {
                this.TempData["Error"] = $"Number of org parameters does not match the number of repo parameters. Orgs: {string.Join(", ", org)}. Repos: {string.Join(", ", repo)}";
                return this.RedirectToAction("Error", "Home");
            }
            SearchIndexViewModel model = new SearchIndexViewModel
            {
                Repositories = await this.GetRepositoriesSelectList().ConfigureAwait(false),
                Query = query,
                IsRegex = isRegex
            };
            IReadOnlyCollection<RepositoryQueryParameter> parameters = RepositoryQueryParameter.ConvertFromArrays(org, repo);

            ManifestQueryResultViewModel queryResultViewModel = await this.GetQueryResultViewModel(parameters, query, isRegex).ConfigureAwait(false);
            model.Result = queryResultViewModel;
            return this.View("Index", model);
        }

        private async Task<ManifestQueryResultViewModel> GetQueryResultViewModel(IReadOnlyCollection<RepositoryQueryParameter> parameters, string query, bool isRegex)
        {
            ManifestQueryResult result = await this.repositoryService.GetCurrentProjects(parameters, query, isRegex).ConfigureAwait(false);
            ManifestQueryResultViewModel queryResultViewModel = this.mapper.Map<ManifestQueryResultViewModel>(result);
            queryResultViewModel.ProjectsTable = new ProjectsTableModel(this.mapper.Map<List<ProjectInfoViewModel>>(result.Projects), this.IsMultipleRepos(parameters));
            return queryResultViewModel;
        }

        private bool IsMultipleRepos(IReadOnlyCollection<RepositoryQueryParameter> parameters)
        {
            if (parameters.Count > 1)
            {
                return true;
            }

            if (parameters.Any(x => x.RepositoryName == "*"))
            {
                return true;
            }

            return false;
        }
    }
}
