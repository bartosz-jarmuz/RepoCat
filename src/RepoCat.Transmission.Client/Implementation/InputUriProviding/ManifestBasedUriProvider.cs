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
    /// <seealso cref="IInputUriProvider" />
    public class ManifestBasedUriProvider : InputUriProviderBase
    {
        protected override string InputUriSuffix { get; } = Strings.ManifestSuffix;

    }
}