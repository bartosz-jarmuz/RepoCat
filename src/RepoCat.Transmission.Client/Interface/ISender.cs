using System.Collections.Generic;
using System.Threading.Tasks;
using RepoCat.Transmission.Models;

namespace RepoCat.Transmission.Client.Interface
{
    /// <summary>
    /// Responsible for sending messages to the API
    /// </summary>
    public interface ISender
    {
        /// <summary>
        /// Sends the specified infos.
        /// </summary>
        /// <param name="infos">The infos.</param>
        /// <returns>Task.</returns>
        Task Send(IEnumerable<ProjectInfo> infos);

        /// <summary>
        /// Sends the specified information.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <returns>Task.</returns>
        Task Send(ProjectInfo info);
    }
}