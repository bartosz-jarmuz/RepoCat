﻿// -----------------------------------------------------------------------
//  <copyright file="RepositoryStampAddingEnricher.cs" company="bartosz.jarmuz@gmail.com">
//   Copyright (c) Bartosz Jarmuż. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Globalization;
using RepoCat.Transmission.Contracts;
using RepoCat.Transmission.Models;

namespace RepoCat.Transmission
{
    public class RepositoryStampAddingEnricher : ProjectInfoEnricherBase
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
        public override void EnrichProjectInfo(string inputUri, ProjectInfo projectInfo, string manifestFilePath,
            object inputObject)
        {
            if (projectInfo != null)
            {
                projectInfo.RepositoryStamp = this.stamp;
            }
        }
    }
}