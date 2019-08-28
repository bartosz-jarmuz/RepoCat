using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RepoCat.Transmitter
{
    public class Worker
    {
        public async Task Work(TransmitterArguments args)
        {
            try
            {
                Program.Log.Info($"Code folder path: [{args.CodeRootFolder}]");
                Program.Log.Info($"Api base URL: [{args.ApiBaseUri}]");
                Program.Log.Info($"Repo: [{args.Repo}]");
                Program.Log.Info($"Repo stamp: [{args.RepoStamp}]");

                LocalProjectUriProvider uriProvider = new LocalProjectUriProvider();
                var uris = uriProvider.GetUris(args.CodeRootFolder);

                ProjectInfoProvider infoProvider = new ProjectInfoProvider();
                var infos = infoProvider.GetInfos(uris, args.Repo, args.RepoStamp);

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