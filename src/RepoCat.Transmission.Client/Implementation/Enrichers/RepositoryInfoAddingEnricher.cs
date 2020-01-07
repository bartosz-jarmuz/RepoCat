using System.Xml.Linq;
using RepoCat.Transmission.Models;

namespace RepoCat.Transmission.Client
{
    public class RepositoryInfoAddingEnricher : EnricherBase
    {
        private readonly TransmitterArguments arguments;

        public RepositoryInfoAddingEnricher(TransmitterArguments arguments)
        {
            this.arguments = arguments;
        }

        ///<inheritdoc cref="IProjectInfoEnricher"/>
        public override void Enrich(ProjectInfo projectInfo, string manifestFilePath, string inputUri)
        {
            if (projectInfo == null) return;
            if (string.IsNullOrWhiteSpace(this.arguments.RepositoryName)) return;
            
            var repoInfo = new RepositoryInfo()
            {
                RepositoryName = this.arguments.RepositoryName,
                OrganizationName = this.arguments.OrganizationName,
                RepositoryMode = this.arguments.RepositoryMode
            };


            if (this.arguments.ManifestCanOverrideRepositoryInfo)
            {
                if (string.IsNullOrEmpty(projectInfo.RepositoryInfo?.RepositoryName))
                {
                    //manifest could prevail, but its not set
                    projectInfo.RepositoryInfo = repoInfo;
                }
            }
            else
            {
                //whatever is in arguments prevails
                projectInfo.RepositoryInfo = repoInfo;
            }
        }
    }
}