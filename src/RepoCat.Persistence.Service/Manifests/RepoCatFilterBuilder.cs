// -----------------------------------------------------------------------
//  <copyright file="RepoCatFilterBuilder.cs" company="bartosz.jarmuz@gmail.com">
//   Copyright (c) Bartosz Jarmuż. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using RepoCat.Persistence.Models;
using RepoCat.Utilities;

namespace RepoCat.Persistence.Service
{
    internal static class RepoCatFilterBuilder
    {

        [SuppressMessage("Globalization", "CA1307:Specify StringComparison", Justification = "Predicate building does not support StringComparison enum")]
        public static FilterDefinition<RepositoryInfo> BuildRepositoryFilter(string organizationName, string repositoryName)
        {
            FilterDefinition<RepositoryInfo> repoNameFilter =
                Builders<RepositoryInfo>.Filter.Where(x =>
                    x.RepositoryName.ToUpperInvariant() == repositoryName.ToUpperInvariant()
                    && x.OrganizationName.ToUpperInvariant() == organizationName.ToUpperInvariant()
                );
            return repoNameFilter;
        }

        [SuppressMessage("Globalization", "CA1307:Specify StringComparison", Justification = "Predicate building does not support StringComparison enum")]
        public static FilterDefinition<SearchStatistics> BuildStatisticsRepositoryFilter(string organizationName, string repositoryName)
        {
            FilterDefinition<SearchStatistics> repoNameFilter =
                Builders<SearchStatistics>.Filter.Where(x =>
                    x.RepositoryName.ToUpperInvariant() == repositoryName.ToUpperInvariant()
                    && x.OrganizationName.ToUpperInvariant() == organizationName.ToUpperInvariant()
                );
            return repoNameFilter;
        }


        public static async Task<FilterDefinition<ProjectInfo>> BuildProjectsFilter(IMongoCollection<ProjectInfo> projects, string query, bool isRegex, RepositoryInfo repo, string stamp = null)
        {
            if (projects == null) throw new ArgumentNullException(nameof(projects));

            FilterDefinition<ProjectInfo> respositoryFilter = Builders<ProjectInfo>.Filter.Empty;
                
            respositoryFilter = AppendRepositoryFilterIfNeeded(respositoryFilter, repo);

            respositoryFilter = await AppendNewestStampFilterIfNeeded(projects, respositoryFilter, repo, stamp).ConfigureAwait(false);

            respositoryFilter = AppendTextFilterIfNeeded(query, isRegex, respositoryFilter);

            return respositoryFilter;
        }

        private static FilterDefinition<ProjectInfo> AppendRepositoryFilterIfNeeded(FilterDefinition<ProjectInfo> filter, RepositoryInfo repo)
        {
            if (repo != null)
            {
                filter = filter & Builders<ProjectInfo>.Filter.Where(x => x.RepositoryId.Equals(repo.Id));
            }
               
            return filter;
        }

        public static FilterDefinition<ProjectInfo> BuildProjectsTextFilter(string query, bool isRegex)
        {
            if (!isRegex)
            {
                return Builders<ProjectInfo>.Filter.Text(query, new TextSearchOptions() { CaseSensitive = false, DiacriticSensitive = false});
            }
            else
            {
                BsonRegularExpression regex = new BsonRegularExpression(new Regex(query,
                    RegexOptions.IgnoreCase));
                return Builders<ProjectInfo>.Filter.Regex(GetComponentFieldName(nameof(ComponentManifest.Tags)),
                           regex)
                       | Builders<ProjectInfo>.Filter.Regex(GetComponentFieldName(nameof(ComponentManifest.Name)),
                           regex)
                       | Builders<ProjectInfo>.Filter.Regex(x => x.AssemblyName, regex)
                       | Builders<ProjectInfo>.Filter.Regex(x => x.ProjectName, regex)
                       | Builders<ProjectInfo>.Filter.Regex(x => x.TargetExtension, regex);
            }
        }


        /// <summary>
        /// If the repository is in snapshot mode, add the filter that selects entries from latest version only
        /// </summary>
        /// <param name="projects"></param>
        /// <param name="filter"></param>
        /// <param name="repository"></param>
        /// <param name="stamp"></param>
        /// <returns></returns>
        private static async Task<FilterDefinition<ProjectInfo>> AppendNewestStampFilterIfNeeded(IMongoCollection<ProjectInfo> projects, 
            FilterDefinition<ProjectInfo> filter, RepositoryInfo repository, string stamp = null)
        {
            if (repository != null && repository.RepositoryMode == RepositoryMode.Snapshot)
            {
                List<string> stamps = await (await projects.DistinctAsync(x => x.RepositoryStamp, filter).ConfigureAwait(false)).ToListAsync().ConfigureAwait(false);
                string stampToInclude;
                if (!string.IsNullOrEmpty(stamp))
                {
                    if (!stamps.Any(x => string.Equals(x, stamp, StringComparison.OrdinalIgnoreCase)))
                    {
                        throw new InvalidOperationException($"Stamp [{stamp}] is not available within the repository");
                    }

                    stampToInclude = stamp;
                }
                else
                {
                    stampToInclude = StampSorter.GetNewestStamp(stamps);
                }

                filter = filter & Builders<ProjectInfo>.Filter.Where(x => x.RepositoryStamp == stampToInclude);
            }

            return filter;
        }
        public static bool CheckIfContainsTextFilter<T>(FilterDefinition<T> filter)
        {
            var serializerRegistry = BsonSerializer.SerializerRegistry;
            var documentSerializer = serializerRegistry.GetSerializer<T>();
            var rendered = filter.Render(documentSerializer, serializerRegistry);
            return rendered.Elements.Any(x => x.Name == "$text");
        }

        private static FilterDefinition<ProjectInfo> AppendTextFilterIfNeeded(string query, bool isRegex, FilterDefinition<ProjectInfo> filter)
        {
            if (!string.IsNullOrEmpty(query) && query != "*")
            {
                filter = filter & BuildProjectsTextFilter(query, isRegex);
            }

            return filter;
        }


        private static string GetComponentFieldName(string field)
        {
            return nameof(ProjectInfo.Components) + "." + field;
        }

    }
}