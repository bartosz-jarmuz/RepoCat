// -----------------------------------------------------------------------
//  <copyright file="ManifestBasedProjectInfoBuilder.cs" company="bartosz.jarmuz@gmail.com">
//   Copyright (c) Bartosz Jarmuż. All rights reserved.
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

        public ManifestBasedProjectInfoBuilder(ILogger logger, IProjectEnrichersFunnel projectEnrichers) : base(logger, projectEnrichers)
        {
            this.logger = logger;
        }

        protected override ProjectInfo GetInfo(string manifestFilePath)
        {
            try
            {
                this.logger.Debug($"Reading Project Info from manifest - {manifestFilePath}");
                XDocument document = XDocument.Load(manifestFilePath);
                
                this.ProjectInfoEnrichers.EnrichManifestXml(manifestFilePath, document, manifestFilePath);

                ProjectInfo info = ManifestDeserializer.DeserializeProjectInfo(document.Root);
                if (info != null)
                {
                    this.ProjectInfoEnrichers.EnrichProject(manifestFilePath, info, manifestFilePath, document);

                    this.logger.Debug($"Loaded project info. {manifestFilePath}.");
                    return info;
                }

            }
            catch (Exception ex)
            {
                this.logger.Error($"Unexpected error while loading project info for [{manifestFilePath}] {ex.Message}. Details in DEBUG mode.");
                this.logger.Debug($"Error details: {ex}.");
            }
            return null;
        }
    }
}