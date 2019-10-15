using System;
using log4net;
using RepoCat.Transmission.Core.Interface;

namespace RepoCat.Transmission.Core.Implementation
{
    /// <summary>
    /// The set of parameters for the worker.
    /// </summary>
    public class TransmitterArguments : ITransmitterArguments
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TransmitterArguments"/> class.
        /// </summary>
        /// <param name="log"></param>
        /// <param name="args">The arguments.</param>
        public TransmitterArguments (ILog log, string[] args)
        {
            try
            {
                this.CodeRootFolder = args[0];
                this.ApiBaseUri = new Uri(args[1]);
                this.RepositoryName = args[2];

                if (args.Length == 4)
                {
                    this.RepositoryStamp = args[3];
                }
                else
                {
                    this.RepositoryStamp = DateTimeOffset.UtcNow.ToString("O");
                }
            }
            catch (Exception ex)
            {
                log.Fatal($"Error while reading arguments. Found following {args.Length} args:");
                foreach (string s in args)
                {
                    log.Fatal(s);
                }
                log.Fatal(ex);
                throw;
            }
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



    }
}