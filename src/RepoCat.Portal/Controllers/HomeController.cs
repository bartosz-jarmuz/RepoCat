using System.Diagnostics;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using RepoCat.Portal.Models;

namespace RepoCat.Portal.Controllers
{
    /// <summary>
    /// Class HomeController.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.Controller" />
    [Route("Home")]
    public class HomeController : Controller
    {
        private readonly IMemoryCache cache;

        /// <summary>
        /// Initializes a new instance of the <see cref="HomeController"/> class.
        /// </summary>
        public HomeController(IMemoryCache memoryCache)
        {
            this.cache = memoryCache;
        }

        /// <summary>
        /// Abouts this instance.
        /// </summary>
        /// <returns>IActionResult.</returns>
        [Route("About")]
        public IActionResult About()
        {
            return this.View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Route("NavHeaderStats")]
        [ResponseCache(Duration = 1000 * 60 * 60)]
        public IActionResult NavHeaderStats()
        {
            return this.ViewComponent("NavHeaderStats");
        }

        [Route("ClearCache")]
        public IActionResult ClearCache()
        {
            
            PropertyInfo prop = this.cache.GetType().GetProperty("EntriesCollection", BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.NonPublic | BindingFlags.Public);
            object innerCache = prop.GetValue(this.cache);
            MethodInfo clearMethod = innerCache.GetType().GetMethod("Clear", BindingFlags.Instance | BindingFlags.Public);
            clearMethod.Invoke(innerCache, null);
            return this.RedirectToAction("Index", "Search");
        }

        /// <summary>
        /// Errors this instance.
        /// </summary>
        /// <returns>IActionResult.</returns>
        [Route("Error")]

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return this.View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier });
        }
    }
}
