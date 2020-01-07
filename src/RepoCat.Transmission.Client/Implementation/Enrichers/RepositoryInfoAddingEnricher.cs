using System.Xml.Linq;
using RepoCat.Transmission.Models;

namespace RepoCat.Transmission.Client
{
    public class RepositoryInfoAddingEnricher : EnricherBase
    {
        private readonly RepositoryInfo repositoryInfo;

        public RepositoryInfoAddingEnricher(RepositoryInfo repositoryInfo)
        {
            this.repositoryInfo = repositoryInfo;
        }

        ///<inheritdoc cref="IProjectInfoEnricher"/>
        public override void Enrich(ProjectInfo projectInfo, string manifestFilePath, string inputUri)
        {
            if (projectInfo != null)
            {
                projectInfo.RepositoryInfo = this.repositoryInfo;
            }
        }
    }
}