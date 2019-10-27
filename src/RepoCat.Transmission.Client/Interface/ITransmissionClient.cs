using System.Threading.Tasks;
using RepoCat.Transmission.Core.Implementation;

namespace RepoCat.Transmission.Core.Interface
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
        Task Work(TransmitterArguments args);
    }
}