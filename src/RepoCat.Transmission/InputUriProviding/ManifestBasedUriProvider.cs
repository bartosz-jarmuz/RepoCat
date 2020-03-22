// -----------------------------------------------------------------------
//  <copyright file="ManifestBasedUriProvider.cs" company="bartosz.jarmuz@gmail.com">
//   Copyright (c) Bartosz Jarmuż. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RepoCat.Transmission.Contracts;

namespace RepoCat.Transmission
{


    /// <summary>
    /// Class LocalProjectUriProvider.
    /// </summary>
    /// <seealso cref="IInputUriProvider" />
    public class ManifestBasedUriProvider : InputUriProviderBase
    {
        public ManifestBasedUriProvider(ILogger logger) : base(logger)
        {
        }
        protected override IEnumerable<string> GetPaths(DirectoryInfo root)
        {
            return root.EnumerateFiles("*", SearchOption.AllDirectories)
                .Where(x =>
                    x.FullName.EndsWith(Strings.ManifestSuffix, StringComparison.OrdinalIgnoreCase)
                )
                .Select(x => x.FullName);
        }
    }
}