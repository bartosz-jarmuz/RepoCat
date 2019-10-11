using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RepoCat.Transmitter
{

    /// <summary>
    /// Main worker class
    /// </summary>
    public class Worker
    {
        /// <summary>
        /// Entry point for the transmission
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>Task.</returns>
        public async Task Work(TransmitterArguments args)
        {
            try
            {
                Program.Log.Info($"Code folder path: [{args.CodeRootFolder}]");
                Program.Log.Info($"Api base URL: [{args.ApiBaseUri}]");
                Program.Log.Info($"Repo: [{args.RepositoryName}]");
                Program.Log.Info($"Repo stamp: [{args.RepositoryStamp}]");

                LocalProjectUriProvider uriProvider = new LocalProjectUriProvider();
                var uris = uriProvider.GetUris(args.CodeRootFolder);

                ProjectInfoProvider infoProvider = new ProjectInfoProvider();
                var infos = infoProvider.GetInfos(uris, args.RepositoryName, args.RepositoryStamp);

                var sender = new Sender(args.ApiBaseUri);
                await sender.Send(infos);

                Program.Log.Info("All done");
            }
            catch (Exception ex)
            {
                Program.Log.Fatal(ex);
            }
        }
    }
}