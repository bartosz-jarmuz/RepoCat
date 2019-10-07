using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using RepoCat.Persistence.Service;
using RepoCat.Portal.Models;
using RepoCat.Portal.Services;

namespace RepoCat.Portal.ViewComponents
{
    public class RepositoryList : ViewComponent
    {
        private readonly ManifestsService service;

        public RepositoryList(ManifestsService manifestsService)
        {
            this.service = manifestsService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {

            var names = await this.service.GetRepositories();
            var model = new RepositoriesListViewModel {Repositories = names};
            return View("~/Areas/Catalog/Views/Shared/Components/RepositoryList/Default.cshtml", model);
        }
    }
}