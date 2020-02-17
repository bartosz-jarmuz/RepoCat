// -----------------------------------------------------------------------
//  <copyright file="EnricherBase.cs" company="bartosz.jarmuz@gmail.com">
//   Copyright (c) Bartosz Jarmuż. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Xml.Linq;
using RepoCat.Transmission.Models;

namespace RepoCat.Transmission.Contracts
{
    public abstract class ProjectInfoEnricherBase : IProjectInfoEnricher
    {
        public virtual void EnrichProjectInfo(string inputUri, ProjectInfo projectInfo, string manifestFilePath,
            object inputObject)
        {
            //no-op
        }

        public virtual void EnrichManifestXml(string inputUri, XDocument manifestXmlDocument, string manifestFilePath)
        {
            //no-op
        }
    }
}