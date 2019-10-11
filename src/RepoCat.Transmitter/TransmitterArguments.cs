using System;

namespace RepoCat.Transmitter
{
    /// <summary>
    /// The set of parameters for the worker.
    /// </summary>
    public class TransmitterArguments
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TransmitterArguments"/> class.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public TransmitterArguments (string[] args)
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
                Program.Log.Fatal($"Error while reading arguments. Found following {args.Length} args:");
                foreach (string s in args)
                {
                    Program.Log.Fatal(s);
                }
                Program.Log.Fatal(ex);
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