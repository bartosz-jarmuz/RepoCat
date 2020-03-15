// -----------------------------------------------------------------------
//  <copyright file="PropertyViewModel.cs" company="bartosz.jarmuz@gmail.com">
//   Copyright (c) Bartosz Jarmuż. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;

namespace RepoCat.Portal.Areas.Catalog.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class PropertyViewModel
    {
        /// <summary>
        /// Name of the property
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Value of the property
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<string> ValueList { get; set; } = new List<string>();
    }
}