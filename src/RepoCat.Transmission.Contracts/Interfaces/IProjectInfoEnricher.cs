// -----------------------------------------------------------------------
//  <copyright file="IProjectInfoEnricher.cs" company="bartosz.jarmuz@gmail.com">
//   Copyright (c) Bartosz Jarmuż. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Xml.Linq;
using RepoCat.Transmission.Models;

namespace RepoCat.Transmission.Contracts
{
    /// <summary>
    /// Adds additional data to project infos
    /// </summary>
    public interface IProjectInfoEnricher
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputUri">Path to the file based on which the project info was identified so far</param>
        /// <param name="projectInfo">An instance of the project info created so far</param>
        /// <param name="manifestFilePath">Path to the manifest file</param>
        /// <param name="inputObject">Object based on which the project info was created</param>
        void EnrichProjectInfo(string inputUri, ProjectInfo projectInfo, string manifestFilePath, object inputObject);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputUri">Path to the file based on which the project info was identified so far</param>
        /// <param name="manifest">Manifest XDocument </param>
        /// <param name="manifestFilePath">Path to the manifest file</param>
        void EnrichManifestXml(string inputUri, XDocument manifest, string manifestFilePath);
    }
}