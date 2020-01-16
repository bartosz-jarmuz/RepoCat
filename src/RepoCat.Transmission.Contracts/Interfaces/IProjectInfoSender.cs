using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RepoCat.Transmission.Models;

namespace RepoCat.Transmission.Contracts
{
    /// <summary>
    /// Responsible for sending messages to the API
    /// </summary>
    public interface IProjectInfoSender
    {
        /// <summary>
        /// Sends the specified infos.
        /// </summary>
        /// <param name="infos">The infos.</param>
        /// <returns>Task.</returns>
        Task<RepositoryImportResult> Send(IEnumerable<ProjectInfo> infos);

        /// <summary>
        /// Sends the specified information.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <returns>Task.</returns>
        Task<ProjectImportResult> Send(ProjectInfo info);

        /// <summary>
        /// Set the base URL of the RepoCat API
        /// </summary>
        /// <param name="baseAddress"></param>
        void SetBaseAddress(Uri baseAddress);
    }
}