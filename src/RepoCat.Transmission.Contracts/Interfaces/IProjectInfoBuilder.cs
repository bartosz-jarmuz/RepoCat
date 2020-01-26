using System.Collections.Generic;
using RepoCat.Transmission.Models;

namespace RepoCat.Transmission.Contracts
{
    public interface IProjectInfoBuilder
    {
        IList<IProjectInfoEnricher> ProjectInfoEnrichers { get; }
        IEnumerable<ProjectInfo> GetInfos(IEnumerable<string> uris);
    }
}