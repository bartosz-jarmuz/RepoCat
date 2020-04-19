// -----------------------------------------------------------------------
//  <copyright file="ProjectsTableModel.cs" company="bartosz.jarmuz@gmail.com">
//   Copyright (c) Bartosz Jarmuż. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RepoCat.Portal.Areas.Catalog.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class ProjectsTableModel
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="projects"></param>
        /// <param name="isMultipleRepositories"></param>
        public ProjectsTableModel(List<ProjectInfoViewModel> projects, bool isMultipleRepositories, bool isSearchResult)
        {
            this.Projects = projects.OrderByDescending(x=>x.SearchAccuracyScore).ToList();
            this.IsMultipleRepositories = isMultipleRepositories;
            this.IsSearchResult = isSearchResult;
            this.BuildPropertiesDictionary();
            this.Repositories = string.Join("_+_", this.Projects.Select(x => $"{x.OrganizationName}_{x.RepositoryName}").Distinct());
        }

        /// <summary>
        /// Repos in this table
        /// </summary>
        public string Repositories { get;  }

        
        private void BuildPropertiesDictionary()
        {
            foreach (var propertiesInProjects in this.Projects.Select(x => x.Properties).Concat(this.Projects.SelectMany(x=>x.Components.Select(c=>c.Properties))))
            {
                foreach (var propertyInProject in propertiesInProjects)
                {
                    var propertyInViewModel = this.Properties.FirstOrDefault(p => p.Key == propertyInProject.Key);
                    
                    if (propertyInViewModel == null)
                    {
                        this.AddNewProperty(propertyInProject);
                    }
                    else
                    {
                        UpdateExistingProperty(propertyInViewModel, propertyInProject);
                    }
                }
            }

            this.Properties = this.Properties.OrderByDescending(x => x.OccurenceCount).ToList();

            foreach (PropertyFilterModel propertyModel in this.Properties)
            {
                propertyModel.Values = propertyModel.Values.OrderByDescending(x => x.OccurenceCount).ToList();
            }
        }

        private static void UpdateExistingProperty(PropertyFilterModel propertyInViewModel, PropertyViewModel propertyInProject)
        {
            propertyInViewModel.OccurenceCount++;
            if (propertyInProject.ValueList != null && propertyInProject.ValueList.Any())
            {
                foreach (string propertyValue in propertyInProject.ValueList.Distinct())
                {
                    IncrementValueInExistingProperty(propertyInViewModel, propertyValue);
                }
            }
            else
            {
                IncrementValueInExistingProperty(propertyInViewModel, propertyInProject.Value);
            }
        }

        private static void IncrementValueInExistingProperty(PropertyFilterModel propertyInViewModel, string propertyValue)
        {
            PropertyFilterValue existingValue = propertyInViewModel.Values.FirstOrDefault(x => x.Value == propertyValue);
            if (existingValue == null)
            {
                propertyInViewModel.Values.Add(new PropertyFilterValue(propertyValue));
            }
            else
            {
                existingValue.OccurenceCount++;
            }
        }

        private void AddNewProperty(PropertyViewModel propertyInProject)
        {
            var values = new List<PropertyFilterValue>();
            if (propertyInProject.ValueList != null && propertyInProject.ValueList.Any())
            {
                values.AddRange(propertyInProject.ValueList.Distinct().Select(x => new PropertyFilterValue(x)));
            }
            else
            {
                values.Add(new PropertyFilterValue(propertyInProject.Value));
            }

            var prop = new PropertyFilterModel()
            {
                Key = propertyInProject.Key,
                Values = values,
                OccurenceCount = 1,
            };

            this.Properties.Add(prop);
        }

        /// <summary>
        /// Gets or sets the manifests.
        /// </summary>
        /// <value>The manifests.</value>
        public List<ProjectInfoViewModel> Projects { get; internal set; } 
     
        /// <summary>
        /// Specifies whther the table contains projects from multiple repos
        /// </summary>
        public bool IsMultipleRepositories { get; set; }

        /// <summary>
        /// Specify whether the table presents search results or a repository contents lists
        /// </summary>
        public bool IsSearchResult { get; }

        /// <summary>
        /// Collection of all properties and their values to be used in extra filters
        /// </summary>
        public List<PropertyFilterModel> Properties { get; set; } = new List<PropertyFilterModel> ();

        /// <summary>
        /// Filters that are active on the table
        /// </summary>
        public Dictionary<string, List<string>> Filters { get; set; } = new Dictionary<string, List<string>>();
    }
}