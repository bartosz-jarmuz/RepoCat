using System;
using System.Collections.Generic;

namespace RepoCat.Transmission.Client
{
    /// <summary>
    /// The set of parameters for the worker.
    /// </summary>
    public class TransmitterArguments : ParameterSet
    {
        public TransmitterArguments() :base() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="TransmitterArguments"/> class.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public TransmitterArguments (string[] args) : base(args)
        {
         
        }

#pragma warning disable CA2227 // Collection properties should be read only
        public ICollection<string> ProjectPaths { get; set; }
#pragma warning restore CA2227 // Collection properties should be read only

        /// <summary>
        /// Gets or sets the code root folder.
        /// </summary>
        /// <value>The code root folder.</value>
        public string CodeRootFolder { get; set; }
        /// <summary>
        /// Gets or sets the repository name.
        /// </summary>
        /// <value>The repo.</value>
        public string RepositoryName { get; set; }

        /// <summary>
        /// Gets or sets the organization name.
        /// </summary>
        /// <value>The repo.</value>
        public string OrganizationName { get; set; }

        /// <summary>
        /// Gets or sets the repo stamp (a datetime or version of the code base).
        /// </summary>
        /// <value>The repo stamp.</value>
        public string RepositoryStamp { get; set; }
        /// <summary>
        /// Gets or sets the API base URI of the RepoCat instance that the transmitter is supposed to talk to.
        /// </summary>
        /// <value>The API base URI.</value>
        public Uri ApiBaseUri { get; set; }

    }
}