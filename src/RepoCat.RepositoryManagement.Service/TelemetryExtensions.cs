using System.Collections.Generic;
using Microsoft.ApplicationInsights;
using RepoCat.Persistence.Models;
using RepoCat.Telemetry;

namespace RepoCat.RepositoryManagement.Service
{
    public static class TelemetryExtensions
    {
        public static void TrackAdding(this Transmission.Models.ProjectInfo project, TelemetryClient telemetryClient)
        {
            telemetryClient.TrackEvent(Names.AddingProjectInfo, new Dictionary<string, string>()
            {
                {PropertyKeys.ProjectName, project.ProjectName},
                {PropertyKeys.ProjectUri, project.ProjectUri},
                {PropertyKeys.OrganizationName, project.RepositoryInfo?.OrganizationName??"NULL"},
                {PropertyKeys.RepositoryName, project.RepositoryInfo?.RepositoryName??"NULL"}
            });
        }

        public static void TrackUpserted(this ProjectInfo project, TelemetryClient telemetryClient)
        {
            telemetryClient.TrackEvent(Names.UpsertedProject, new Dictionary<string, string>()
            {
                {PropertyKeys.Id, project.Id.ToString()},
                {PropertyKeys.ProjectName, project.ProjectName},
                {PropertyKeys.ProjectUri, project.ProjectUri},
                {PropertyKeys.RepositoryId, project.RepositoryId.ToString()},
            });

        }
        public static void TrackUpserted(this RepositoryInfo repo, TelemetryClient telemetryClient)
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