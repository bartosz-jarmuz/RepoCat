// -----------------------------------------------------------------------
//  <copyright file="UriProviderFactory.cs" company="bartosz.jarmuz@gmail.com">
//   Copyright (c) Bartosz Jarmuż. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using RepoCat.Transmission.Builders.Excel;
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
            else if (args.TransmissionMode == TransmissionMode.ExcelDatabaseBased)
            {
                return new ExcelBasedUriProvider(logger);
            }
            else
            {
                return new ManifestBasedUriProvider(logger);
            }
        }

    }
}