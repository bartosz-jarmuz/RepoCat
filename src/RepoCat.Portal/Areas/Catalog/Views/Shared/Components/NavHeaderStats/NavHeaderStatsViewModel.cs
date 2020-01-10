using System.Collections.Generic;

namespace RepoCat.Portal.Areas.Catalog.Views.Components
{
    /// <summary>
    /// View model for the stats displayed in top bar
    /// </summary>
    public class NavHeaderStatsViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public int OrganizationsCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int RepositoriesCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int ProjectsCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int ComponentsCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int TagsCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int PropertiesCount { get; set; }
    }
}
