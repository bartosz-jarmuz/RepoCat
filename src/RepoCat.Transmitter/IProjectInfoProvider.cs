using System.Collections.Generic;

namespace RepoCat.Transmitter
{
    internal interface IProjectInfoProvider
    {
        ProjectInfo GetInfo(string uri);
        IEnumerable<ProjectInfo> GetInfos(IEnumerable<string> uris);
    }
}