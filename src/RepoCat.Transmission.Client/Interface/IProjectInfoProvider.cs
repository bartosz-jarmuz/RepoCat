using System.Collections.Generic;
using RepoCat.Transmission.Models;

namespace RepoCat.Transmission.Client.Interface
{
    internal interface IProjectInfoProvider
    {
        ProjectInfo GetInfo(string uri, string repo, string repoStamp);
        IEnumerable<ProjectInfo> GetInfos(IEnumerable<string> uris, string repo, string repoStamp);
    }
}