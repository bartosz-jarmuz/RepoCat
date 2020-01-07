using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using RepoCat.Transmission.Models;

namespace RepoCat.Transmission.Client
{
    /// <summary>
    /// Main worker class
    /// </summary>
    public class TransmissionClient : ITransmissionClient
    {
        private readonly ILogger logger;

        /// <summary>
        /// Creates new instance
        /// </summary>
        /// <param name="logger"></param>
        public TransmissionClient(ILogger logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// Entry point for the transmission
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>Task.</returns>
        public Task Work(string[] args)
        {
            TransmitterArguments arguments= new TransmitterArguments(args);
            this.logger.Debug($"Arguments: {arguments.OriginalParameterInputString}");
            return this.Work(arguments);
        }
    

        /// <summary>
        /// Entry point for the transmission
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>Task.</returns>
        public async Task Work(TransmitterArguments args)
        {
            if (args == null) throw new ArgumentNullException(nameof(args));

            try
            {
                this.DisplayParameters(args);

                IEnumerable<string> uris = this.GetPaths(args);

                var infoProvider = ProjectInfoProviderFactory.Get(args, this.logger);
                IEnumerable<ProjectInfo> infos = infoProvider.GetInfos(uris);

                using (HttpSender sender = new HttpSender(args.ApiBaseUri, this.logger))
                {
                    await sender.Send(infos).ConfigureAwait(false);
                }

                this.logger.Info("All done");
            }
            catch (Exception ex)
            {
                this.logger.Fatal(ex);
            }
        }


        private void DisplayParameters(TransmitterArguments args)
        {
            this.logger.Info($"Command line string: [{args.OriginalParameterInputString}]");
            this.logger.Info($"Resolved parameters:");

            foreach (KeyValuePair<string, string> parameter in args.GetParameterCollection())
            {
                this.logger.Info($"{parameter.Key}: [{parameter.Value}]");
            }
        }

        private IEnumerable<string> GetPaths(TransmitterArguments args)
        {
            IEnumerable<string> uris;
            if (args.ProjectPaths == null || !args.ProjectPaths.Any())
            {
                var provider = UriProviderFactory.Get(args);
                uris = provider.GetUris(args.CodeRootFolder);
            }
            else
            {
                uris = args.ProjectPaths;
            }

            return uris;
        }
    }
}