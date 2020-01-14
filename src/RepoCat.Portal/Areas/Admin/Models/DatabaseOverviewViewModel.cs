using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RepoCat.Portal.Areas.Admin.Models
{
    /// <summary>
    /// View model
    /// </summary>
    public class DatabaseOverviewViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public List<CollectionSummary> Collections { get; internal set; }
    }
}
