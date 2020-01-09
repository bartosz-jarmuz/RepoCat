// -----------------------------------------------------------------------
//  <copyright file="UriProviderFactory.cs" company="SDL plc">
//   Copyright (c) SDL plc. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;

namespace RepoCat.Transmission
{
    public static class UriProviderFactory
    {
        public static IInputUriProvider Get(TransmitterArguments args)
        {
            if (args == null) throw new ArgumentNullException(nameof(args));
            if (args.TransmissionMode == TransmissionMode.LocalDotNetProjects)
            {
                return new LocalDotNetProjectUriProvider();
            }
            else
            {
                return new ManifestBasedUriProvider();
            }
        }

    }
}