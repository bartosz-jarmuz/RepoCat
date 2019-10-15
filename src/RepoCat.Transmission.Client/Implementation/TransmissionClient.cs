using System;
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
        public async Task Work(ITransmitterArguments args)
        {
            try
            {
                this.log.Info($"Code folder path: [{args.CodeRootFolder}]");
                this.log.Info($"Api base URL: [{args.ApiBaseUri}]");
                this.log.Info($"Repo: [{args.RepositoryName}]");
                this.log.Info($"Repo stamp: [{args.RepositoryStamp}]");

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