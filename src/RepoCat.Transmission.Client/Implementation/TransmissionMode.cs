// -----------------------------------------------------------------------
//  <copyright file="TransmissionMode.cs" company="SDL plc">
//   Copyright (c) SDL plc. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace RepoCat.Transmission.Client
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
        /// Use when there are no 'projects' (like .csproj files), e.g. for scripts, excel macros etc.
        /// In this case, the transmitter will look for Manifest files directly, rather than analyze the projects
        /// </summary>
        LocalManifestBased
    }
}