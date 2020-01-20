// -----------------------------------------------------------------------
//  <copyright file="RepositoryQueryParameter.cs" company="SDL plc">
//   Copyright (c) SDL plc. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using RepoCat.Persistence.Models;

namespace RepoCat.RepositoryManagement.Service
{
    /// <summary>
    /// A parameter object for repository queries
    /// </summary>
    public class RepositoryQueryParameter
    {
        public RepositoryQueryParameter()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="organizationName"></param>
        /// <param name="repositoryName"></param>
        public RepositoryQueryParameter(string organizationName, string repositoryName)
        {
            this.OrganizationName = organizationName;
            this.RepositoryName = repositoryName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="repositoryInfo"></param>
        public RepositoryQueryParameter(RepositoryInfo repositoryInfo)
        {
            if (repositoryInfo == null) throw new ArgumentNullException(nameof(repositoryInfo));

            this.OrganizationName = repositoryInfo.OrganizationName;
            this.RepositoryName = repositoryInfo.RepositoryName;
        }

        /// <summary>
        /// Builds parameters from arrays
        /// </summary>
        /// <param name="orgs"></param>
        /// <param name="repos"></param>
        /// <returns></returns>
        public static IReadOnlyCollection<RepositoryQueryParameter> ConvertFromArrays(string[] orgs, string[] repos)
        {
            if (orgs == null) throw new ArgumentNullException(nameof(orgs));
            if (repos == null) throw new ArgumentNullException(nameof(repos));
            if (orgs.Length != repos.Length)
            {
                throw new InvalidOperationException(
                    $"Number of org parameters does not match the number of repo parameters. Orgs: {string.Join(", ", orgs)}. Repos: {string.Join(", ", repos)}");
            }
            var collection = new List<RepositoryQueryParameter>();
            for (int i = 0; i < orgs.Length; i++)
            {
                collection.Add(new RepositoryQueryParameter(orgs[i], repos[i]));
            }

            return collection.AsReadOnly();
        }

        public static string ConvertToQueryString(IEnumerable<RepositoryQueryParameter> parameters)
        {
            var sb = new StringBuilder();
            foreach (RepositoryQueryParameter parameter in parameters)
            {
                sb.Append($"&org={parameter.OrganizationName}&repo={parameter.RepositoryName}");
            }

            return "?"+ sb.ToString().TrimStart('&');
        }


        /// <summary>
        /// 
        /// </summary>
        public string OrganizationName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string RepositoryName { get; set; }
    }
}