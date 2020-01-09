using System.Collections.Generic;
using System.Linq;

namespace RepoCat.Transmission.Client
{
    public class RepositoryImportResult
    {
        public int SuccessCount => this.ProjectResults.Count(x => x.Success);
        public int FailedCount=> this.ProjectResults.Count(x =>!x.Success);
        public IReadOnlyCollection<ProjectImportResult> ProjectResults { get; set;} = new List<ProjectImportResult>();
    }
}