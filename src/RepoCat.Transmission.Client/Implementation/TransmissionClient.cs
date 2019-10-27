using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using log4net;
using RepoCat.Transmission.Core.Interface;

namespace RepoCat.Transmission.Core.Implementation
{
    /// <summary>
    /// Main worker class
    /// </summary>
    public class TransmissionClient : ITransmissionClient
    {
        private readonly ILog log;

        /// <summary>
        /// Creates new instance
        /// </summary>
        /// <param name="log"></param>
        public TransmissionClient(ILog log)
        {
            this.log = log;
        }

        /// <summary>
        /// Entry point for the transmission
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>Task.</returns>
        public Task Work(string[] args)
        {
            TransmitterArguments arguments= new TransmitterArguments(args);
            this.log.Debug($"Arguments: {arguments.OriginalParameterInputString}");
            return this.Work(arguments);
        }
    

        /// <summary>
        /// Entry point for the transmission
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>Task.</returns>
        public async Task Work(TransmitterArguments args)
        {
            try
            {
                foreach (KeyValuePair<string, string> parameter in args.OriginalParameterCollection)
                {
                    this.log.Info($"{parameter.Key}: [{parameter.Value}]");
                }

                LocalProjectUriProvider uriProvider = new LocalProjectUriProvider();
                var uris = uriProvider.GetUris(args.CodeRootFolder);

                ProjectInfoProvider infoProvider = new ProjectInfoProvider(this.log);
                var infos = infoProvider.GetInfos(uris, args.RepositoryName, args.RepositoryStamp);

                var sender = new HttpSender(args.ApiBaseUri, this.log);
                await sender.Send(infos);

                this.log.Info("All done");
            }
            catch (Exception ex)
            {
                this.log.Fatal(ex);
            }
        }
    }
}