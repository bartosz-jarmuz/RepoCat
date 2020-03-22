// -----------------------------------------------------------------------
//  <copyright file="LocalDotNetProjectUriProvider.cs" company="bartosz.jarmuz@gmail.com">
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
    public class LocalDotNetProjectUriProvider : InputUriProviderBase
    {
        private string Csproj { get; } = ".csproj";
        private string SqlProj { get; } = ".sqlproj";


        protected override IEnumerable<string> GetPaths(DirectoryInfo root)
        {
            return root.EnumerateFiles("*", SearchOption.AllDirectories)
                .Where(x => 
                    x.FullName.EndsWith(this.Csproj, StringComparison.OrdinalIgnoreCase)||
                    x.FullName.EndsWith(this.SqlProj, StringComparison.OrdinalIgnoreCase)
                )
                .Select(x => x.FullName);
        }

        public LocalDotNetProjectUriProvider(ILogger logger) : base(logger)
        {

        }
    }
}