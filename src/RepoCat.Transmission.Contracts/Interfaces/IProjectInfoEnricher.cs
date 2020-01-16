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
        /// <param name="projectInfo">An instance of the project info created so far</param>
        /// <param name="manifestFilePath">Path to the manifest file</param>
        /// <param name="inputUri">Path to the file based on which the project info was identified so far</param>
        void Enrich(ProjectInfo projectInfo, string manifestFilePath, string inputUri);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="manifest">Manifest XDocument </param>
        /// <param name="manifestFilePath">Path to the manifest file</param>
        /// <param name="inputUri">Path to the file based on which the project info was identified so far</param>
        void Enrich(XDocument manifest, string manifestFilePath, string inputUri);
    }
}