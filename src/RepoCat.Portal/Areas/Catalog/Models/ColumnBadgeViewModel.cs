// -----------------------------------------------------------------------
//  <copyright file="ColumnBadgeViewModel.cs" company="bartosz.jarmuz@gmail.com">
//   Copyright (c) Bartosz Jarmuż. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace RepoCat.Portal.Areas.Catalog.Models
{
    /// <summary>
    /// Class TagBadgeViewModel.
    /// </summary>
    public class ColumnBadgeViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        /// Extra css to add
        /// </summary>
        public string AdditionalClass { get; set; }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnBadgeViewModel"/> class.
        /// </summary>
        public ColumnBadgeViewModel(string columnName, string additionalClass)
        {
            this.ColumnName = columnName;
            this.AdditionalClass = additionalClass;
        }
      
    }
}
