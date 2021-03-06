﻿// -----------------------------------------------------------------------
//  <copyright file="TelemetryExtensions.cs" company="bartosz.jarmuz@gmail.com">
//   Copyright (c) Bartosz Jarmuż. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
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

        public static void TrackDeleteRepository(this TelemetryClient telemetryClient, string organizationName, string repositoryName)
        {
            telemetryClient.TrackEvent(Names.DeleteRepository, new Dictionary<string, string>()
            {
                {PropertyKeys.OrganizationName, organizationName},
                {PropertyKeys.RepositoryName, repositoryName},
            });
        }


        public static void TrackSearch(this TelemetryClient telemetryClient, IReadOnlyCollection<RepositoryQueryParameter> parameters, string query, bool isRegex, int resultsCount, TimeSpan elapsed)
        {
            foreach (RepositoryQueryParameter repositoryQueryParameter in parameters)
            {
                telemetryClient.TrackEvent(Names.RepositorySearch, new Dictionary<string, string>()
                {
                    {PropertyKeys.OrganizationName, repositoryQueryParameter.OrganizationName},
                    {PropertyKeys.RepositoryName, repositoryQueryParameter.RepositoryName},
                    {PropertyKeys.Query, query },
                    {PropertyKeys.IsRegex, isRegex.ToString()},
                    {PropertyKeys.ResultsCount, resultsCount.ToString()},
                    {PropertyKeys.QueryExecutionTime, elapsed.TotalMilliseconds.ToString()},
                });
            }
            
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
            telemetryClient.TrackEvent(Names.PostingProjectInfo, new Dictionary<string, string>()
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

        public static void TrackCleanupJob(this TelemetryClient telemetryClient, SnapshotRepoCleanupResult result, long elapsedMiliseconds)
        {
            telemetryClient.TrackEvent(Names.RepositoryBatchCleanupJobFinished, new Dictionary<string, string>()
            {
                {PropertyKeys.ExecutionTime, elapsedMiliseconds.ToString()},
                {PropertyKeys.RepositoriesCount, result.RepositoryResults.Count.ToString()},
                {PropertyKeys.ProjectsCleanedCount, result.RepositoryResults.Sum(x=>x.Value).ToString()},

            });

            foreach (KeyValuePair<RepositoryInfo, long> repositoryResult in result.RepositoryResults)
            {
                telemetryClient.TrackEvent(Names.RepositoryCleanup, new Dictionary<string, string>()
                {
                    {PropertyKeys.OrganizationName, repositoryResult.Key.OrganizationName},
                    {PropertyKeys.RepositoryName, repositoryResult.Key.RepositoryName},
                    {PropertyKeys.ProjectsCleanedCount, repositoryResult.Value.ToString()},
                });
            }
        }


    }
}