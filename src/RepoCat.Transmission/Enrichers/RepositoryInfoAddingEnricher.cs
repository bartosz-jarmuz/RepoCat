// -----------------------------------------------------------------------
//  <copyright file="RepositoryInfoAddingEnricher.cs" company="bartosz.jarmuz@gmail.com">
//   Copyright (c) Bartosz Jarmuż. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using RepoCat.Transmission.Contracts;
using RepoCat.Transmission.Models;

namespace RepoCat.Transmission
{
    public class RepositoryInfoAddingEnricher : ProjectInfoEnricherBase
    {
        private readonly TransmitterArguments arguments;

        public RepositoryInfoAddingEnricher(TransmitterArguments arguments)
        {
            this.arguments = arguments;
        }

        ///<inheritdoc cref="IProjectInfoEnricher"/>
        public override void EnrichProjectInfo(string inputUri, ProjectInfo projectInfo, string manifestFilePath,
            object inputObject)
        {
            if (projectInfo == null) return;
            
            var repoInfo = new RepositoryInfo()
            {
                RepositoryName = this.arguments.RepositoryName,
                OrganizationName = this.arguments.OrganizationName,
                RepositoryMode = this.arguments.RepositoryMode
            };

            if (!string.IsNullOrEmpty(repoInfo.RepositoryName))
            {
                if (this.arguments.ManifestCanOverrideRepositoryInfo)
                {
                    if (string.IsNullOrEmpty(projectInfo.RepositoryInfo?.RepositoryName))
                    {
                        //manifest could prevail (as it is the current value), but its not set
                        projectInfo.RepositoryInfo = repoInfo;
                    }
                }
                else
                {
                    //whatever is in arguments prevails
                    projectInfo.RepositoryInfo = repoInfo;
                }
            }

            var currentOrg = projectInfo.RepositoryInfo?.OrganizationName;
            var currentRepo = projectInfo.RepositoryInfo?.RepositoryName;
            if (string.IsNullOrEmpty(currentRepo) || string.IsNullOrEmpty(currentOrg))
            {
                projectInfo.RepositoryInfo = new RepositoryInfo();
                projectInfo.RepositoryInfo.RepositoryName = string.IsNullOrEmpty(currentRepo) ? "Unspecified" : currentRepo;
                projectInfo.RepositoryInfo.OrganizationName = string.IsNullOrEmpty(currentOrg) ? "Unspecified" : currentOrg;
            }
        }
    }
}