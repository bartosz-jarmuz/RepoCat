// -----------------------------------------------------------------------
//  <copyright file="ManifestQueryResultSorter.cs" company="bartosz.jarmuz@gmail.com">
//   Copyright (c) Bartosz Jarmuż. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using RepoCat.Persistence.Models;
using RepoCat.Utilities;

namespace RepoCat.RepositoryManagement.Service
{
    public class ManifestQueryResultSorter
    {
        private static class ScoreMultipliers
        {
            public static readonly decimal ProjectName = 2;
            public static readonly decimal Tags = 1.5M;
            public static readonly decimal Description = 1;
        }


        public IEnumerable<KeyValuePair<Project, decimal>> Sort(IEnumerable<Project> projects, IEnumerable<string> searchTokens)
        {
            var sortedCollection = new List<KeyValuePair<Project, decimal>>();
            var enumeratedTokens = searchTokens.ToList();
            foreach (Project sortedProject in  projects)
            {
                var score = this.GetProjectScore(sortedProject, enumeratedTokens);
                sortedCollection.Add(new KeyValuePair<Project, decimal>(sortedProject, score));
            }
            
            return sortedCollection.OrderByDescending(x=>x.Value);
        }

        private decimal GetProjectScore(Project project, IReadOnlyCollection<string> tokens)
        {
            decimal score = 0;

            score += this.GetStringScore(project.ProjectInfo.ProjectName, ScoreMultipliers.ProjectName, tokens);
            score += this.GetStringScore(project.ProjectInfo.AssemblyName, ScoreMultipliers.ProjectName, tokens);

            score += this.GetStringScore(project.ProjectInfo.Tags, ScoreMultipliers.Tags, tokens);


            score += this.GetStringScore(project.ProjectInfo.ProjectDescription, ScoreMultipliers.Description, tokens);

            return score;
        }

        private decimal GetStringScore(IEnumerable<string> toSearch, decimal scoreMultiplier, IReadOnlyCollection<string> tokens)
        {
            decimal score = 0;
            foreach (string searchString in toSearch)
            {
                score += this.GetStringScore(searchString, scoreMultiplier, tokens);
            }

            return score;
        }

        private decimal GetStringScore(string toSearch, decimal scoreMultiplier, IEnumerable<string> tokens)
        {
            const decimal equalsMultiplier = 2;
            const decimal startsWithMultiplier = 1.5M;
            const decimal containsMultiplier = 1;
            decimal score = 0;

            foreach (string token in tokens)
            {
                if (toSearch.Equals(token, StringComparison.OrdinalIgnoreCase))
                {
                    score += 1 * scoreMultiplier * equalsMultiplier;
                }
                else if (toSearch.StartsWith(token, StringComparison.OrdinalIgnoreCase))
                {
                    score += 1 * scoreMultiplier * startsWithMultiplier;
                }
                else if (toSearch.Contains(token, StringComparison.OrdinalIgnoreCase))
                {
                    score += 1 * scoreMultiplier * containsMultiplier;
                }
            }

            return score;
        }


    }
}