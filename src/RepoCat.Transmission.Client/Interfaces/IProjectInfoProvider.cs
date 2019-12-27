using System.Collections.Generic;
using RepoCat.Transmission.Models;

namespace RepoCat.Transmission.Client
{
    internal interface IProjectInfoProvider
    {
        ProjectInfo GetInfo(string uri, RepositoryInfo repositoryInfo, string repoStamp);
        IEnumerable<ProjectInfo> GetInfos(IEnumerable<string> uris, RepositoryInfo repositoryInfo, string repoStamp);
    }
}