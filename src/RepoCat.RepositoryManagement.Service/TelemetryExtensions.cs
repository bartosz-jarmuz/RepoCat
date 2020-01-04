using System.Collections.Generic;
using Microsoft.ApplicationInsights;
using RepoCat.Persistence.Models;
using RepoCat.Telemetry;

namespace RepoCat.RepositoryManagement.Service
{
    public static class TelemetryExtensions
    {

        public static void TrackViewRepository(this TelemetryClient telemetryClient, string organizationName, string repositoryName)
        {
            telemetryClient.TrackEvent(Names.ViewRepository, new Dictionary<string, string>()
            {
                {PropertyKeys.OrganizationName, organizationName},
                {PropertyKeys.RepositoryName, repositoryName},
            });
        } 
        
        public static void TrackSearch(this TelemetryClient telemetryClient, string organizationName, string repositoryName, string query, bool isRegex)
        {
            telemetryClient.TrackEvent(Names.RepositorySearch, new Dictionary<string, string>()
            {
                {PropertyKeys.OrganizationName, organizationName},
                {PropertyKeys.RepositoryName, repositoryName},
                {PropertyKeys.Query, query },
                {PropertyKeys.IsRegex, isRegex.ToString()},
            });
        }

        public static void TrackFileDownload(this TelemetryClient telemetryClient, ProjectInfo project, bool isLocal, long? fileSize = null)
        {
            telemetryClient.TrackEvent(Names.FileDownload, new Dictionary<string, string>()
            {
                {PropertyKeys.ProjectName, project.ProjectName},
                {PropertyKeys.ProjectUri, project.ProjectUri},
                {PropertyKeys.IsLocal, isLocal.ToString() },
                {PropertyKeys.FileSize, fileSize?.ToString() },
            });
        }




        public static void TrackAdding(this TelemetryClient telemetryClient, Transmission.Models.ProjectInfo project)
        {
            telemetryClient.TrackEvent(Names.AddingProjectInfo, new Dictionary<string, string>()
            {
                {PropertyKeys.ProjectName, project.ProjectName},
                {PropertyKeys.ProjectUri, project.ProjectUri},
                {PropertyKeys.OrganizationName, project.RepositoryInfo?.OrganizationName??"NULL"},
                {PropertyKeys.RepositoryName, project.RepositoryInfo?.RepositoryName??"NULL"}
            });
        }

        public static void TrackUpserted(this TelemetryClient telemetryClient, ProjectInfo project)
        {
            telemetryClient.TrackEvent(Names.UpsertedProject, new Dictionary<string, string>()
            {
                {PropertyKeys.Id, project.Id.ToString()},
                {PropertyKeys.ProjectName, project.ProjectName},
                {PropertyKeys.ProjectUri, project.ProjectUri},
                {PropertyKeys.RepositoryId, project.RepositoryId.ToString()},
            });

        }
        public static void TrackUpserted(this TelemetryClient telemetryClient, RepositoryInfo repo)
        {
            telemetryClient.TrackEvent(Names.UpsertedRepository, new Dictionary<string, string>()
            {
                {PropertyKeys.OrganizationName, repo.OrganizationName},
                {PropertyKeys.RepositoryName, repo.RepositoryName },
                {PropertyKeys.Id, repo.Id.ToString()},
                {PropertyKeys.RepositoryMode, repo.RepositoryMode.ToString()},
            });
        }

       
    }
}