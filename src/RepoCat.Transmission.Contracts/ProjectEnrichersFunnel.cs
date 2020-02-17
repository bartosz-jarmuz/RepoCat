// -----------------------------------------------------------------------
//  <copyright file="ProjectEnrichersFunnel.cs" company="bartosz.jarmuz@gmail.com">
//   Copyright (c) Bartosz Jarmuż. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using RepoCat.Transmission.Models;

namespace RepoCat.Transmission.Contracts
{
    public class ProjectEnrichersFunnel : IProjectEnrichersFunnel
    {
        private IList<IProjectInfoEnricher> ProjectInfoEnrichers { get; } = new List<IProjectInfoEnricher>();

        public void Add(IProjectInfoEnricher enricher)
        {
            this.ProjectInfoEnrichers.Add(enricher);
        }

        public void EnrichManifestXml(string inputUri, XDocument manifest, string manifestFilePath)
        {
            foreach (IProjectInfoEnricher projectInfoEnricher in this.ProjectInfoEnrichers)
            {
                if (manifest != null)
                {
                    projectInfoEnricher.EnrichManifestXml(inputUri, manifest, manifestFilePath);
                }
            }
        }

        public void EnrichProject(string projectUri, ProjectInfo projectInfo, string manifestFilePath, object inputObject)
        {
            foreach (IProjectInfoEnricher projectInfoEnricher in this.ProjectInfoEnrichers)
            {
                if (projectInfo != null)
                {
                    projectInfoEnricher.EnrichProjectInfo(projectUri, projectInfo, manifestFilePath, inputObject);
                }
            }
        }

        public IEnumerable<string> GetNames()
        {
            return this.ProjectInfoEnrichers.Select(x => x.GetType().Name);
        }
    }
}