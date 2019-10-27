using System;
using System.ComponentModel;
using DotNetLittleHelpers;
using log4net;
using RepoCat.Transmission.Core.Interface;
using RepoCat.Transmission.Models;

namespace RepoCat.Transmission.Core.Implementation
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
        /// Gets or sets the repo stamp (a datetime or version of the code base).
        /// </summary>
        /// <value>The repo stamp.</value>
        public string RepositoryStamp { get; set; }
        /// <summary>
        /// Gets or sets the API base URI of the RepoCat instance that the transmitter is supposed to talk to.
        /// </summary>
        /// <value>The API base URI.</value>
        public Uri ApiBaseUri { get; set; }

        /// <summary>
        /// Gets or sets the mode of project info transmission
        /// </summary>
        public RepositoryMode RepositoryMode { get; set; }

    }
}