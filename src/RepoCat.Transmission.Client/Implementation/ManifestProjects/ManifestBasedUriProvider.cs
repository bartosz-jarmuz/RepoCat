// -----------------------------------------------------------------------
//  <copyright file="ManifestBasedUriProvider.cs" company="SDL plc">
//   Copyright (c) SDL plc. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RepoCat.Transmission.Client
{
    /// <summary>
    /// Class LocalProjectUriProvider.
    /// </summary>
    /// <seealso cref="IProjectUriProvider" />
    public class ManifestBasedUriProvider : IProjectUriProvider
    {
        /// <summary>
        /// Finds the URIs of the projects in a local (file system) directory 
        /// </summary>
        /// <param name="rootUri">The root URI.</param>
        /// <returns>IEnumerable&lt;System.String&gt;.</returns>
        public IEnumerable<string> GetUris(string rootUri)
        {
            var codeDirectory = new DirectoryInfo(rootUri);
            if (codeDirectory.Exists)
            {
                return codeDirectory.EnumerateFiles("*", SearchOption.AllDirectories)
                    .Where(x => x.FullName.EndsWith(Strings.ManifestSuffix, StringComparison.OrdinalIgnoreCase))
                    .Select(x => x.FullName);
            }

            return Array.Empty<string>();
        }
    }
}