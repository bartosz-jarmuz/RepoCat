using System.Collections.Generic;
using RepoCat.Transmission.Models;

namespace RepoCat.Transmission.Client
{
    internal interface IProjectInfoProvider
    {
        ProjectInfo GetInfo(string uri, string organization, string repo, string repoStamp);
        IEnumerable<ProjectInfo> GetInfos(IEnumerable<string> uris, string organization, string repo, string repoStamp);
    }
}