// -----------------------------------------------------------------------
//  <copyright file="UriProviderFactory.cs" company="SDL plc">
//   Copyright (c) SDL plc. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using RepoCat.Transmission.Contracts;

namespace RepoCat.Transmission
{
    public static class UriProviderFactory
    {
        public static IInputUriProvider Get(TransmitterArguments args, ILogger logger)
        {
            if (args == null) throw new ArgumentNullException(nameof(args));
            if (args.TransmissionMode == TransmissionMode.LocalDotNetProjects)
            {
                return new LocalDotNetProjectUriProvider(logger);
            }
            else
            {
                return new ManifestBasedUriProvider(logger);
            }
        }

    }
}