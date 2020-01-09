using System.Collections.Generic;
using RepoCat.Transmission.Models;

namespace RepoCat.Transmission.Client
{
    public interface IProjectInfoBuilder
    {
        IList<IProjectInfoEnricher> ProjectInfoEnrichers { get; }
        ProjectInfo GetInfo(string projectUri);
        IEnumerable<ProjectInfo> GetInfos(IEnumerable<string> uris);
    }
}