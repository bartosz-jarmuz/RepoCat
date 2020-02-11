// -----------------------------------------------------------------------
//  <copyright file="DatabaseOverviewViewModel.cs" company="bartosz.jarmuz@gmail.com">
//   Copyright (c) Bartosz Jarmuż. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;

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
