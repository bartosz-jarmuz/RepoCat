// -----------------------------------------------------------------------
//  <copyright file="IProjectEnrichersFunnel.cs" company="bartosz.jarmuz@gmail.com">
//   Copyright (c) Bartosz Jarmuż. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Xml.Linq;
using RepoCat.Transmission.Models;

namespace RepoCat.Transmission.Contracts
{
    public interface IProjectEnrichersFunnel
    {
        void Add(IProjectInfoEnricher enricher);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputUri">Path to the file based on which the project info was identified so far</param>
        /// <param name="manifest">Manifest XDocument </param>
        /// <param name="manifestFilePath">Path to the manifest file</param>
        void EnrichManifestXml(string inputUri, XDocument manifest, string manifestFilePath);
       
        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputUri">Path to the file based on which the project info was identified so far</param>
        /// <param name="projectInfo">An instance of the project info created so far</param>
        /// <param name="manifestFilePath">Path to the manifest file</param>
        void EnrichProject(string inputUri, ProjectInfo projectInfo, string manifestFilePath);


        IEnumerable<string> GetNames();
    }
}