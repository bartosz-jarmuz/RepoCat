﻿using System;
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

                this.ValidateParameters(args);

                IEnumerable<string> uris = GetPaths(args);

                DotNetProjectInfoProvider infoProvider = new DotNetProjectInfoProvider(this.logger);

                var repoInfo = new RepositoryInfo()
                {
                    RepositoryName = args.RepositoryName,
                    OrganizationName = args.OrganizationName,
                    RepositoryMode = args.RepositoryMode
                };

                IEnumerable<ProjectInfo> infos = infoProvider.GetInfos(uris, repoInfo, args.RepositoryStamp);

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

        private static IEnumerable<string> GetPaths(TransmitterArguments args)
        {
            IEnumerable<string> uris;
            if (args.ProjectPaths == null || !args.ProjectPaths.Any())
            {
                LocalDotNetProjectUriProvider uriProvider = new LocalDotNetProjectUriProvider();
                uris = uriProvider.GetUris(args.CodeRootFolder);
            }
            else
            {
                uris = args.ProjectPaths;
            }

            return uris;
        }

        private void ValidateParameters(TransmitterArguments args)
        {
            if (string.IsNullOrEmpty(args.RepositoryStamp))
            {
                args.RepositoryStamp = DateTime.UtcNow.ToString("O", CultureInfo.InvariantCulture);
                this.logger.Info($"Repository stamp was null or empty - updated to current execution time (UTC) - [{args.RepositoryStamp}]");
            }
        }
    }
}