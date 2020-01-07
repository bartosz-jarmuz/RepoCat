// -----------------------------------------------------------------------
//  <copyright file="RepositoryStampAddingEnricher.cs" company="SDL plc">
//   Copyright (c) SDL plc. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Globalization;
using RepoCat.Transmission.Models;

namespace RepoCat.Transmission.Client
{
    public class RepositoryStampAddingEnricher : EnricherBase
    {
        private readonly string stamp;
        private readonly ILogger logger;

        public RepositoryStampAddingEnricher(string stamp, ILogger logger)
        {
            this.stamp = stamp;
            this.logger = logger;
        }

        ///<inheritdoc cref="IProjectInfoEnricher"/>
        public override void Enrich(ProjectInfo projectInfo, string manifestFilePath, string inputUri)
        {
            if (projectInfo != null)
            {
                if (string.IsNullOrEmpty(this.stamp))
                {
                    projectInfo.RepositoryStamp = DateTime.UtcNow.ToString("O", CultureInfo.InvariantCulture);
                    this.logger.Info($"Repository stamp was null or empty - updated to current execution time (UTC) - [{projectInfo.RepositoryStamp}]");
                }
                else
                {
                    projectInfo.RepositoryStamp = this.stamp;
                }

            }
        }
    }
}