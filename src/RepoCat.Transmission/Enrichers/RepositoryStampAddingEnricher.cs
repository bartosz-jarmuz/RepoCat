// -----------------------------------------------------------------------
//  <copyright file="RepositoryStampAddingEnricher.cs" company="SDL plc">
//   Copyright (c) SDL plc. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Globalization;
using RepoCat.Transmission.Contracts;
using RepoCat.Transmission.Models;

namespace RepoCat.Transmission
{
    public class RepositoryStampAddingEnricher : EnricherBase
    {
        private readonly string stamp;

        public RepositoryStampAddingEnricher(string stamp, ILogger logger)
        {
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            if (string.IsNullOrEmpty(stamp))
            {
                this.stamp = DateTime.UtcNow.ToString("O", CultureInfo.InvariantCulture);
                logger.Info($"Repository stamp was null or empty - updated to current execution time (UTC) - [{this.stamp}]");
            }
            else
            {
                this.stamp = stamp;
            }
        }

        ///<inheritdoc cref="IProjectInfoEnricher"/>
        public override void Enrich(ProjectInfo projectInfo, string manifestFilePath, string inputUri)
        {
            if (projectInfo != null)
            {
                projectInfo.RepositoryStamp = this.stamp;
            }
        }
    }
}