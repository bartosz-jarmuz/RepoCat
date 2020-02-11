// -----------------------------------------------------------------------
//  <copyright file="PropertyFilterValue.cs" company="bartosz.jarmuz@gmail.com">
//   Copyright (c) Bartosz Jarmuż. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace RepoCat.Portal.Areas.Catalog.Models
{
    /// <summary>
    /// Represents a single property value
    /// </summary>
    public class PropertyFilterValue
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public PropertyFilterValue(string value)
        {
            this.Value = value;
        }

        /// <summary>
        /// The value
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// How many of the projects have this property value
        /// </summary>
        public int OccurenceCount { get; set; } = 1;
    }
}