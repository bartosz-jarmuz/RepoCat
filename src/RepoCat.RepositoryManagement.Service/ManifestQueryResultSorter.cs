// -----------------------------------------------------------------------
//  <copyright file="ManifestQueryResultSorter.cs" company="bartosz.jarmuz@gmail.com">
//   Copyright (c) Bartosz Jarmuż. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using RepoCat.Persistence.Models;

namespace RepoCat.RepositoryManagement.Service
{
    public class ManifestQueryResultSorter : IManifestQueryResultSorter
    {
        private static class ScoreMultipliers
        {
            public const decimal Equal = 10;
            public const decimal TokenizedEqual = 6;
            public const decimal StartsWith = 4;
            public const decimal Contains = 2;
            
            public const decimal Name = 7;
            public const decimal Tags = 1.5M;
            public const decimal Description = 1;
            public const decimal Properties = 1;
        }


        public IEnumerable<Project> Sort(IEnumerable<Project> projects, IEnumerable<string> searchTokens)
        {
            var sortedCollection = new List<Project>();
            var enumeratedTokens = searchTokens.ToList();
            foreach (Project sortedProject in projects)
            {
                var score = this.GetProjectScore(sortedProject, enumeratedTokens);
                sortedProject.SearchAccuracyScore = score;
                sortedCollection.Add(sortedProject);
            }

            return sortedCollection.OrderByDescending(x => x.SearchAccuracyScore);
        }

        private decimal GetProjectScore(Project project, IReadOnlyCollection<string> tokens)
        {
            decimal score = 0;

            score += this.GetAssemblyNameScores(project.ProjectInfo, tokens);

            score += this.GetStringScore(project.ProjectInfo.ProjectName, ScoreMultipliers.Name, tokens);
            score += this.GetStringScore(project.ProjectInfo.Tags, ScoreMultipliers.Tags, tokens);
            score += this.GetStringScore(project.ProjectInfo.Owner, ScoreMultipliers.Tags, tokens);
            score += this.GetStringScore(project.ProjectInfo.Properties, ScoreMultipliers.Tags, tokens);
            score += this.GetStringScore(project.ProjectInfo.ProjectDescription, ScoreMultipliers.Description, tokens);

            if (project.ProjectInfo.Components != null)
            {
                foreach (ComponentManifest component in project.ProjectInfo.Components)
                {
                    score += this.GetStringScore(component.Name, ScoreMultipliers.Name, tokens);
                    score += this.GetStringScore(component.Tags, ScoreMultipliers.Tags, tokens);
                    score += this.GetStringScore(component.Properties, ScoreMultipliers.Tags, tokens);
                    score += this.GetStringScore(component.Description, ScoreMultipliers.Description, tokens);
                }
            }

            return score;
        }

        private decimal GetStringScore(PropertiesCollection projectInfoProperties, in decimal scoreBaseValue, IReadOnlyCollection<string> tokens)
        {
            var values = new List<string>();
            foreach (Property projectInfoProperty in projectInfoProperties)
            {
                if (projectInfoProperty.ValueList != null && projectInfoProperty.ValueList.Any())
                {
                    values.AddRange(projectInfoProperty.ValueList);
                }
                else
                {
                    values.Add(projectInfoProperty.Value);
                }
            }

            return this.GetStringScore(values, scoreBaseValue, tokens);
        }

        private decimal GetStringScore(IEnumerable<string> toSearch, decimal scoreBaseValue, IReadOnlyCollection<string> tokens)
        {
            decimal score = 0;
            foreach (string searchString in toSearch)
            {
                score += this.GetStringScore(searchString, scoreBaseValue, tokens);
            }

            return score;
        }

        private decimal GetStringScore(string toSearch, decimal scoreBaseValue, IEnumerable<string> tokens)
        {
            if (string.IsNullOrWhiteSpace(toSearch))
            {
                return 0;
            }
            decimal score = 0;
            List<string> tokenizedSearchPhrase = SearchInputTokenizer.Tokenize(toSearch);
            foreach (string token in tokens)
            {
                if (toSearch.Equals(token, StringComparison.OrdinalIgnoreCase))
                {
                    score += scoreBaseValue * ScoreMultipliers.Equal;
                }
                else if (this.TokenizedEquals(tokenizedSearchPhrase, token))
                {
                    score += scoreBaseValue * ScoreMultipliers.TokenizedEqual;
                }
                else if (toSearch.StartsWith(token, StringComparison.OrdinalIgnoreCase))
                {
                    if (IsSufficientTokenLength(token))
                    {
                        decimal bonus = GetPercentageBonus(scoreBaseValue * ScoreMultipliers.StartsWith, token, toSearch);
                        score += (scoreBaseValue * ScoreMultipliers.StartsWith) + bonus;
                    }
                }
                else if (toSearch.Contains(token, StringComparison.OrdinalIgnoreCase) && IsSufficientTokenLength(token))
                {
                    decimal bonus = GetPercentageBonus(scoreBaseValue * ScoreMultipliers.Contains, token, toSearch);
                    score += (scoreBaseValue * ScoreMultipliers.Contains) + bonus;
                }
            }

            return score;
        }

        /// <summary>
        /// Handle proj
        /// ect name scoring separately because of the 'with/without extension'
        /// </summary>
        /// <param name="projectInfo"></param>
        /// <param name="tokens"></param>
        /// <returns></returns>
        private decimal GetAssemblyNameScores(ProjectInfo projectInfo, IReadOnlyCollection<string> tokens)
        {
            const decimal scoreBaseValue = ScoreMultipliers.Name;
            if (projectInfo.AssemblyName == null)
            {
                return 0;
            }
            decimal score = 0;
            var tokenizedSearchPhrase = SearchInputTokenizer.Tokenize(projectInfo.AssemblyName);
            foreach (string token in tokens)
            {

                if (projectInfo.AssemblyName.Equals(token, StringComparison.OrdinalIgnoreCase)
                || Path.GetFileNameWithoutExtension(projectInfo.AssemblyName).Equals(token, StringComparison.OrdinalIgnoreCase)
                )
                {
                    score += scoreBaseValue * ScoreMultipliers.Equal;
                }
                else if (this.TokenizedEquals(tokenizedSearchPhrase, token))
                {
                    score += scoreBaseValue * ScoreMultipliers.TokenizedEqual;
                }

                else if (projectInfo.AssemblyName.StartsWith(token, StringComparison.OrdinalIgnoreCase)
                || Path.GetFileNameWithoutExtension(projectInfo.AssemblyName).StartsWith(token, StringComparison.OrdinalIgnoreCase)
                )
                {
                    if (IsSufficientTokenLength(token))
                    {
                        decimal bonus = GetPercentageBonus(scoreBaseValue * ScoreMultipliers.StartsWith, token, projectInfo.AssemblyName);
                        score += (scoreBaseValue * ScoreMultipliers.StartsWith) + bonus;
                    }
                }
                else if ((projectInfo.AssemblyName.Contains(token, StringComparison.OrdinalIgnoreCase)
                          || Path.GetFileNameWithoutExtension(projectInfo.AssemblyName).Contains(token, StringComparison.OrdinalIgnoreCase)) && IsSufficientTokenLength(token)
                )
                {
                    decimal bonus = GetPercentageBonus(scoreBaseValue * ScoreMultipliers.Contains, token, projectInfo.AssemblyName);
                    score += (scoreBaseValue * ScoreMultipliers.Contains) + bonus;
                }
            }

            return score;
        }


     

        private bool TokenizedEquals(List<string> tokenizedSearchPhrase, string token)
        {
            return tokenizedSearchPhrase.Any(x => x.Equals(token, StringComparison.OrdinalIgnoreCase));
        }

        private static decimal GetPercentageBonus(decimal baseValue, string token, string toSearch)
        {
            decimal percentage = token.Length / (decimal)toSearch.Length;
            if (percentage < 0.1M)
            {
                return (baseValue * -1); //if the matched substring is absolutely tiny, do not add any score (i.e. add penalty equal to score)
            }
            if (percentage < 0.2M)
            {
                return (baseValue *-1 ) + baseValue * percentage ; //if the matched substring is only very insignificant, add a penalty instead of bonus
            }
            return baseValue * percentage;
        }

        private static bool IsSufficientTokenLength(string token)
        {
            return token.Length >= 3;
        }

        
    }

    /// <summary>
    /// Tokenizes the input for the search sorter
    /// </summary>
    public static class SearchInputTokenizer
    {
        public static List<string> Tokenize(string toSearch)
        {
            if (toSearch.Length <= 3)
            {
                return new List<string>() { toSearch };
            }
            //larger than 3 is a fudge factor number that should eliminate a number of typical falsely high scores from file extensions etc
            List<string> tokens = toSearch.Split(new[] { '.', ' ', '-', ',', '!', '?', ';', ':', '|', '—','–', '\\', '_', '/' }, StringSplitOptions.RemoveEmptyEntries)
                .Where(x => x.Length > 3).ToList();


            return tokens.SelectMany(SplitPascalCase).ToList();
        }

        private static IEnumerable<string> RegexSplitPascalCase(string str)
        {
            return Regex.Replace(str, "[a-z][A-Z]", m => $"{m.Value[0]} {m.Value[1]}").Split(' ');
        }

        private static bool IsUpperOrDigit(char input)
        {
            return char.IsUpper(input) || char.IsDigit(input);
        }

        private  static IEnumerable<string> SplitPascalCase(string input)
        {
            var tokens = new List<string>();

            if (!string.IsNullOrEmpty(input))
            {
                var sb = new StringBuilder();
                sb.Append(input[0]);

                for (int i = 1; i < input.Length; i++)
                {
                    char currentChar = input[i];
                    char previousChar = input[i - 1];
                    // any time we hit an uppercase OR number, it's a new word
                    if (IsUpperOrDigit(currentChar) && !IsUpperOrDigit(previousChar))
                    {
                        //unless its something like NETCore
                        tokens.Add(sb.ToString());
                        sb.Clear();
                    }

                    // add regularly
                    sb.Append(currentChar);
                    if (i == input.Length - 1)
                    {
                        tokens.Add(sb.ToString());
                    }
                }

            }

            return tokens;
        }
        
    }
}