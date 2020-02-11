// -----------------------------------------------------------------------
//  <copyright file="PropertyBadgeViewModel.cs" company="bartosz.jarmuz@gmail.com">
//   Copyright (c) Bartosz Jarmuż. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace RepoCat.Portal.Areas.Catalog.Models
{
    /// <summary>
    /// Class TagBadgeViewModel.
    /// </summary>
    public class PropertyBadgeViewModel
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
        /// 
        /// </summary>
        public string ImageClass { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool InludeAddRemoveIcon { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyBadgeViewModel"/> class.
        /// </summary>
        public PropertyBadgeViewModel(PropertyFilterModel propertyFilter, string additionalClass, string imageClass, bool inludeAddRemoveIcon = true)
        {
            this.PropertyFilter = propertyFilter;
            this.AdditionalClass = additionalClass;
            this.ImageClass = imageClass;
            this.InludeAddRemoveIcon = inludeAddRemoveIcon;
        }
      
    }
}
