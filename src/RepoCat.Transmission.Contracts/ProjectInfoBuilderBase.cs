﻿// -----------------------------------------------------------------------
//  <copyright file="ProjectInfoProviderBase.cs" company="SDL plc">
//   Copyright (c) SDL plc. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using RepoCat.Transmission.Models;

namespace RepoCat.Transmission.Contracts
{
    public abstract class ProjectInfoBuilderBase : IProjectInfoBuilder
    {
        private readonly ILogger logger;

        protected ProjectInfoBuilderBase(ILogger logger)
        {
            this.logger = logger;
        }

        public IList<IProjectInfoEnricher> ProjectInfoEnrichers { get; } = new List<IProjectInfoEnricher>();
        public abstract ProjectInfo GetInfo(string projectUri);

        public virtual IEnumerable<ProjectInfo> GetInfos(IEnumerable<string> uris)
        {
            if (uris == null) throw new ArgumentNullException(nameof(uris));

            int counter = 0;
            int skippedCounter = 0;
            foreach (string uri in uris)
            {
                this.logger.Debug($"Checking file #{counter}. {uri}");

                ProjectInfo info = this.GetInfo(uri);
                if (info != null)
                {
                    counter++;
                    yield return info;
                }
                else
                {
                    skippedCounter++;
                    this.logger.Debug($"File #{counter} does not contain a valid manifest. {uri}");
                }
            }
            this.logger.Info($"Loaded project infos for {counter} files. Did not load {skippedCounter}.");

        }
    }
}