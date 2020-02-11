// -----------------------------------------------------------------------
//  <copyright file="IRepositoryMonitoringSettings.cs" company="bartosz.jarmuz@gmail.com">
//   Copyright (c) Bartosz Jarmuż. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;

namespace RepoCat.Portal.RecurringJobs
{
    /// <summary>
    /// Collection of settings
    /// </summary>
    public interface IRepositoryMonitoringSettings
    {
        /// <summary>
        /// A collection of settings for each monitored repository
        /// </summary>\
        #pragma warning disable CA2227 
        List<RepositoryToScanSettings> RepositorySettings { get; set; }
#pragma warning restore CA2227

    }
}