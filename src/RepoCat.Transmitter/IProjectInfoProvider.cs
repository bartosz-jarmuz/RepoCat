using System.Collections.Generic;

namespace RepoCat.Transmitter
{
    internal interface IProjectInfoProvider
    {
        ProjectInfo GetInfo(string uri, string repo, string repoStamp);
        IEnumerable<ProjectInfo> GetInfos(IEnumerable<string> uris, string repo, string repoStamp);
    }
}