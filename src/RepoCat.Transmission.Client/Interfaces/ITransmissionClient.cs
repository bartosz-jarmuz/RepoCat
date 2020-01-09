using System.Collections.Generic;
using System.Threading.Tasks;

namespace RepoCat.Transmission.Client
{
    /// <summary>
    /// Controls end-to-end transmission of manifests to API
    /// </summary>
    public interface ITransmissionClient
    {
        /// <summary>
        /// Additional enrichers can be added to the pipeline of ProjectInfo creation, so that newly created project info can be updated with some extra properties<br/>
        /// E.g. assembly size, last contributor, last commit date etc.
        /// </summary>
        IList<IProjectInfoEnricher> AdditionalProjectInfoEnrichers { get; }

        /// <summary>
        /// Performs the transmission - first gathers the manifest files, then builds the project info, then sends that over to the API.<br/>
        /// It's possible to add optional custom IProjectInfoEnrichers in order to append custom properties to the project info before it's sent
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <param name="uriProvider">Provider of URIs to projects containing Manifests or Manifests themselves.<br/>If left null, a default implementation will be created based on arguments.</param>
        /// <param name="projectInfoBuilder">Creates the ProjectInfo based on the input files - RepoCat manifest XMLs and optionally other projects.<br/>If left null, a default implementation will be created based on arguments.</param>
        /// <returns>Task.</returns>
        Task<RepositoryImportResult> Work(TransmitterArguments args, IInputUriProvider uriProvider = null, IProjectInfoBuilder projectInfoBuilder = null);
    }
}