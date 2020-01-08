using System.Threading.Tasks;

namespace RepoCat.Transmission.Client
{
    /// <summary>
    /// Controls end-to-end transmission of manifests to API
    /// </summary>
    public interface ITransmissionClient
    {
        /// <summary>
        /// Entry point for the transmission
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>Task.</returns>
        Task<RepositoryImportResult> Work(TransmitterArguments args);
    }
}