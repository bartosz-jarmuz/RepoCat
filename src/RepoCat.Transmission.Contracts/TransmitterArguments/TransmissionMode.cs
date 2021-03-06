﻿// -----------------------------------------------------------------------
//  <copyright file="TransmissionMode.cs" company="bartosz.jarmuz@gmail.com">
//   Copyright (c) Bartosz Jarmuż. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace RepoCat.Transmission.Contracts
{
    /// <summary>
    /// Specify how the transmitter should operate.
    /// There can be different approaches - local net projects, cloud repository java projects, local scripts etc
    /// </summary>
    public enum TransmissionMode
    {
        /// <summary>
        /// Use when Transmitter is supposed to have access to .NET project files via local paths
        /// </summary>
        LocalDotNetProjects,

        /// <summary>
        /// Use when there are no 'projects' (like .csproj files), e.g. for scripts, excel macros etc.<br/>
        /// In this case, the transmitter will look for Manifest files directly, rather than analyze the projects
        /// </summary>
        LocalManifestBased,

        /// <summary>
        /// Get data from an excel file which contains info about multiple projects
        /// </summary>
        ExcelDatabaseBased,

    }
}