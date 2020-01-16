// -----------------------------------------------------------------------
//  <copyright file="ProjectInfoProviderFactory.cs" company="SDL plc">
//   Copyright (c) SDL plc. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using RepoCat.Transmission.Contracts;
using RepoCat.Transmission.Models;

namespace RepoCat.Transmission
{
    public static class ProjectInfoBuilderFactory
    {

        public static IProjectInfoBuilder Get(TransmitterArguments args, ILogger logger)
        {
            if (args == null) throw new ArgumentNullException(nameof(args));

            IProjectInfoBuilder infoBuilder;
            if (args.TransmissionMode == TransmissionMode.LocalDotNetProjects)
            {
                infoBuilder = new DotNetProjectInfoBuilder(logger);
            }
            else
            {
                infoBuilder = new ManifestBasedProjectInfoBuilder(logger);
                infoBuilder.ProjectInfoEnrichers.Add(new RelativePathResolvingEnricher());
            }
            AddGenericEnrichersToProvider(args, infoBuilder, logger);
            return infoBuilder;
        }

        private static void AddGenericEnrichersToProvider(TransmitterArguments args, IProjectInfoBuilder infoBuilder, ILogger logger)
        {
            infoBuilder.ProjectInfoEnrichers.Add(new RepositoryStampAddingEnricher(args.RepositoryStamp, logger));
            infoBuilder.ProjectInfoEnrichers.Add(new RepositoryInfoAddingEnricher(args));
        }
    }
}