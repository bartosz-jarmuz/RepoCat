﻿// -----------------------------------------------------------------------
//  <copyright file="ManifestBasedProjectInfoProvider.cs" company="SDL plc">
//   Copyright (c) SDL plc. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Xml.Linq;
using RepoCat.Serialization;
using RepoCat.Transmission.Contracts;
using RepoCat.Transmission.Models;

namespace RepoCat.Transmission
{
    /// <summary>
    /// Provides project info based on Manifest files, without parsing a project file
    /// </summary>
    public class ManifestBasedProjectInfoBuilder : ProjectInfoBuilderBase
    {
        private readonly ILogger logger;

        public ManifestBasedProjectInfoBuilder(ILogger logger) : base(logger)
        {
            this.logger = logger;
        }

        public override ProjectInfo GetInfo(string projectUri)
        {
            try
            {
                this.logger.Debug($"Reading Project Info - {projectUri}");
                XDocument document = XDocument.Load(projectUri);
                foreach (IProjectInfoEnricher projectInfoEnricher in this.ProjectInfoEnrichers)
                {
                    projectInfoEnricher.Enrich(document, projectUri, projectUri);
                }

                ProjectInfo info = ManifestDeserializer.DeserializeProjectInfo(document.Root);
                if (info != null)
                {
                    foreach (IProjectInfoEnricher projectInfoEnricher in this.ProjectInfoEnrichers)
                    {
                        projectInfoEnricher.Enrich(info, projectUri, projectUri);
                    }

                    this.logger.Debug($"Loaded project info. {projectUri}.");
                    return info;
                }

            }
            catch (Exception ex)
            {
                this.logger.Error($"Unexpected error while loading project info for [{projectUri}] {ex.Message}. Details in DEBUG mode.");
                this.logger.Debug($"Error details: {ex}.");
            }
            return null;
        }
    }
}