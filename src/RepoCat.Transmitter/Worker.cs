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
            Console.WriteLine($"Code folder path: [{codeFolderPath}]");
            Console.WriteLine($"Api base URL: [{codeFolderPath}]");

            LocalProjectUriProvider uriProvider = new LocalProjectUriProvider();
            List<string> uris = uriProvider.GetUris(codeFolderPath).ToList();
            Console.WriteLine($"Found {uris.Count} project URIs");

            ProjectInfoProvider infoProvider = new ProjectInfoProvider();
            List<ProjectInfo> infos = infoProvider.GetInfos(uris).ToList();
            Console.WriteLine($"Loaded {infos.Count} project infos");

            var sender = new Sender(null);
            var tasks = new List<Task>();
            foreach (ProjectInfo projectInfo in infos)
            {
                Console.WriteLine($"Sending {projectInfo.GetName()} project info");

                tasks.Add(sender.Send(projectInfo));
            }

            await Task.WhenAll(tasks);
        }
    }
}