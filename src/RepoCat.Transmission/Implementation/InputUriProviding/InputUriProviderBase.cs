// -----------------------------------------------------------------------
//  <copyright file="InputUriProviderBase.cs" company="SDL plc">
//   Copyright (c) SDL plc. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace RepoCat.Transmission
{
    public abstract class InputUriProviderBase : IInputUriProvider
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1056:Uri properties should not be strings", Justification = "It's a suffix")]
        protected abstract string InputUriSuffix { get; }

        public virtual IEnumerable<string> GetUris(string rootUri,  Regex ignoredPathsRegex = null)
        {
            
            var root = new DirectoryInfo(rootUri);
            if (root.Exists)
            {
                var paths = root.EnumerateFiles("*", SearchOption.AllDirectories)
                    .Where(x => x.FullName.EndsWith(this.InputUriSuffix, StringComparison.OrdinalIgnoreCase))
                    .Select(x => x.FullName);
                if (ignoredPathsRegex != null)
                {
                    return paths.Where(path => !ignoredPathsRegex.IsMatch(path));
                }

                return paths;
            }
            return Array.Empty<string>();
        }
    }
}