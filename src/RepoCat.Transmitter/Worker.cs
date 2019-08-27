using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RepoCat.Transmitter
{
    public class Worker
    {
        public async Task Work(string codeFolderPath, string baseApiAddress)
        {
            Program.Log.Info($"Code folder path: [{codeFolderPath}]");
            Program.Log.Info($"Api base URL: [{codeFolderPath}]");

            LocalProjectUriProvider uriProvider = new LocalProjectUriProvider();
            var uris = uriProvider.GetUris(codeFolderPath);
             Program.Log.Debug($"Finished loading project URIs");

            ProjectInfoProvider infoProvider = new ProjectInfoProvider();
            var infos = infoProvider.GetInfos(uris).ToList();
            Program.Log.Info($"Loaded {infos.Count} project infos.");

            var sender = new Sender(null);
            var tasks = new List<Task>();
            foreach (ProjectInfo projectInfo in infos)
            {

                tasks.Add(sender.Send(projectInfo));
            }

            await Task.WhenAll(tasks);

            Program.Log.Info("All done");
        }
    }
}