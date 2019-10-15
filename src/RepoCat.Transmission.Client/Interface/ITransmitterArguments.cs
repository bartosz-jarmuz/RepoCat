using System;

namespace RepoCat.Transmission.Core.Interface
{
    /// <summary>
    /// Arguments needed to launch the transmitter
    /// </summary>
    public interface ITransmitterArguments
    {
        /// <summary>
        /// Gets or sets the code root folder.
        /// </summary>
        /// <value>The code root folder.</value>
        string CodeRootFolder { get; set; }

        /// <summary>
        /// Gets or sets the repository name.
        /// </summary>
        /// <value>The repo.</value>
        string RepositoryName { get; set; }

        /// <summary>
        /// Gets or sets the repo stamp (a datetime or version of the code base).
        /// </summary>
        /// <value>The repo stamp.</value>
        string RepositoryStamp { get; set; }

        /// <summary>
        /// Gets or sets the API base URI of the RepoCat instance that the transmitter is supposed to talk to.
        /// </summary>
        /// <value>The API base URI.</value>
        Uri ApiBaseUri { get; set; }
    }
}