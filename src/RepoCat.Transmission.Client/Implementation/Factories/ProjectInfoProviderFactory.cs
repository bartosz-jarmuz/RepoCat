// -----------------------------------------------------------------------
//  <copyright file="ProjectInfoProviderFactory.cs" company="SDL plc">
//   Copyright (c) SDL plc. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using RepoCat.Transmission.Models;

namespace RepoCat.Transmission.Client
{
    public static class ProjectInfoProviderFactory
    {

        public static IProjectInfoProvider Get(TransmitterArguments args, ILogger logger)
        {
            if (args == null) throw new ArgumentNullException(nameof(args));

            IProjectInfoProvider infoProvider;
            if (args.TransmissionMode == TransmissionMode.LocalDotNetProjects)
            {
                infoProvider = new DotNetProjectInfoProvider(logger);
            }
            else
            {
                infoProvider = new ManifestBasedProjectInfoProvider(logger);
                infoProvider.ProjectInfoEnrichers.Add(new RelativePathResolvingEnricher());
            }
            AddGenericEnrichersToProvider(args, infoProvider, logger);
            return infoProvider;
        }

        private static void AddGenericEnrichersToProvider(TransmitterArguments args, IProjectInfoProvider infoProvider, ILogger logger)
        {
            infoProvider.ProjectInfoEnrichers.Add(new RepositoryStampAddingEnricher(args.RepositoryStamp, logger));

            if (args.RepositoryName != null)
            {
                var repoInfo = new RepositoryInfo()
                {
                    RepositoryName = args.RepositoryName,
                    OrganizationName = args.OrganizationName,
                    RepositoryMode = args.RepositoryMode
                };
                infoProvider.ProjectInfoEnrichers.Add(new RepositoryInfoAddingEnricher(repoInfo));
            }
        }
    }
}