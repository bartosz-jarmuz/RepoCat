// -----------------------------------------------------------------------
//  <copyright file="ProjectInfoBuilderFactory.cs" company="bartosz.jarmuz@gmail.com">
//   Copyright (c) Bartosz Jarmuż. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using RepoCat.Transmission.Builders.Excel;
using RepoCat.Transmission.Contracts;

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
                infoBuilder = new DotNetProjectInfoBuilder(logger, args);
            }
            else if (args.TransmissionMode == TransmissionMode.ExcelDatabaseBased)
            {
                infoBuilder = new ExcelBasedProjectInfoBuilder(logger, args.PropertyMappings);
            }
            else
            {
                infoBuilder = new ManifestBasedProjectInfoBuilder(logger);
                infoBuilder.ProjectInfoEnrichers.Add(new RelativePathResolvingEnricher());
                infoBuilder.ProjectInfoEnrichers.Add(new AssemblyInfoResolvingEnricher());
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