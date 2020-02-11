// -----------------------------------------------------------------------
//  <copyright file="PropertyFilterModel.cs" company="bartosz.jarmuz@gmail.com">
//   Copyright (c) Bartosz Jarmuż. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;

namespace RepoCat.Portal.Areas.Catalog.Models
{
    /// <summary>
    /// Represents a single project property
    /// </summary>
    public class PropertyFilterModel
    {
        /// <summary>
        /// The name of the property
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// The collection of values for this property across all projects
        /// </summary>
        public List<PropertyFilterValue> Values { get; set; } = new List<PropertyFilterValue>();

        /// <summary>
        /// How many of the projects have this property defined
        /// </summary>
        public int OccurenceCount { get; set; }
    }
}