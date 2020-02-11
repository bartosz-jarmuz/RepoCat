// -----------------------------------------------------------------------
//  <copyright file="EnricherBase.cs" company="bartosz.jarmuz@gmail.com">
//   Copyright (c) Bartosz Jarmuż. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Xml.Linq;
using RepoCat.Transmission.Models;

namespace RepoCat.Transmission.Contracts
{
    public abstract class EnricherBase : IProjectInfoEnricher
    {
        public virtual void Enrich(ProjectInfo projectInfo, string manifestFilePath, string inputUri)
        {
            //no-op
        }

        public virtual void Enrich(XDocument manifestXmlDocument, string manifestFilePath, string inputUri)
        {
            //no-op
        }
    }
}