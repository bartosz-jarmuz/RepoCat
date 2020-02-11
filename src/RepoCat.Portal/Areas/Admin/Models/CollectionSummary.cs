// -----------------------------------------------------------------------
//  <copyright file="CollectionSummary.cs" company="bartosz.jarmuz@gmail.com">
//   Copyright (c) Bartosz Jarmuż. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Globalization;
using Humanizer;

namespace RepoCat.Portal.Areas.Admin.Models
{
    /// <summary>
    /// Summary info about a collection
    /// </summary>
    public class CollectionSummary
    {
        /// <summary>
        /// Name
        /// </summary>
        public string CollectionNamespace { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public long SizeInBytes { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string CollectionSize()
        {
            return this.SizeInBytes.Bytes().Humanize("#.##",CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// 
        /// </summary>
        public long AverageObjectSizeInBytes { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string AverageObjectSize()
        {
            return this.AverageObjectSizeInBytes.Bytes().Humanize("#.##", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// 
        /// </summary>
        public long DocumentCount { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public long IndexesCount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public long TotalIndexSizeInBytes { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string TotalIndexSize()
        {
            return this.TotalIndexSizeInBytes.Bytes().Humanize("#.##", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// 
        /// </summary>
        public long StorageSizeInBytes { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string StorageSize()
        {
            return this.StorageSizeInBytes.Bytes().Humanize("#.##", CultureInfo.InvariantCulture);
        }
    }

    
}