using System.Collections.Generic;
using RepoCat.RepositoryManagement.Service;

namespace RepoCat.Portal.Areas.Catalog.Models
{
    /// <summary>
    /// Class TagBadgeViewModel.
    /// </summary>
    public class PropertyFilterBadgeViewModel
    {
        /// <summary>
        /// The filter
        /// </summary>
        public PropertyFilterModel PropertyFilter { get; set; }

        /// <summary>
        /// Extra css to add
        /// </summary>
        public string AdditionalClass { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TagBadgeViewModel"/> class.
        /// </summary>
        public PropertyFilterBadgeViewModel(PropertyFilterModel propertyFilter, string additionalClass)
        {
            this.PropertyFilter = propertyFilter;
            this.AdditionalClass = additionalClass;
        }
      
    }
}
