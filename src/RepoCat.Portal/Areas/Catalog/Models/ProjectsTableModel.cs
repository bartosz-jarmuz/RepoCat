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
        public ProjectsTableModel(List<ProjectInfoViewModel> projects, bool isMultipleRepositories)
        {
            this.Projects = projects;
            this.IsMultipleRepositories = isMultipleRepositories;
            this.BuildPropertiesDictionary();
            this.Repositories = string.Join("_+_", this.Projects.Select(x => $"{x.OrganizationName}_{x.RepositoryName}").Distinct());
        }

        /// <summary>
        /// Repos in this table
        /// </summary>
        public string Repositories { get;  }

        private static bool IsList(object propertyValue, out List<string> list)
        {
            list = null;
            if (propertyValue.GetType() != typeof(string))
            {
                if (propertyValue is IEnumerable enumerable)
                {
                    list = new List<string>();
                    foreach (object o in enumerable)
                    {
                        list.Add(o.ToString());
                    }
                    return true;
                }
            }

            return false;
        }

        private void BuildPropertiesDictionary()
        {
            foreach (Dictionary<string, object> propertiesInProjects in this.Projects.Select(x => x.Properties).Concat(this.Projects.SelectMany(x=>x.Components.Select(c=>c.Properties))))
            {
                foreach (KeyValuePair<string, object> propertyInProject in propertiesInProjects)
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

        private static void UpdateExistingProperty(PropertyFilterModel propertyInViewModel, KeyValuePair<string, object> propertyInProject)
        {
            propertyInViewModel.OccurenceCount++;
            if (IsList(propertyInProject.Value, out List<string> list))
            {
                foreach (string propertyValue in list)
                {
                    IncrementValueInExistingProperty(propertyInViewModel, propertyValue);
                }
            }
            else
            {
                IncrementValueInExistingProperty(propertyInViewModel, propertyInProject.Value.ToString());
            }
        }

        private static void IncrementValueInExistingProperty(PropertyFilterModel propertyInViewModel, string propertyValue)
        {
            var existingValue = propertyInViewModel.Values.FirstOrDefault(x => x.Value == propertyValue);
            if (existingValue == null)
            {
                propertyInViewModel.Values.Add(new PropertyFilterValue(propertyValue));
            }
            else
            {
                existingValue.OccurenceCount++;
            }
        }

        private void AddNewProperty(KeyValuePair<string, object> propertyInProject)
        {
            var values = new List<PropertyFilterValue>();
            if (IsList(propertyInProject.Value, out List<string> list))
            {
                values.AddRange(list.Select(x => new PropertyFilterValue(x)));
            }
            else
            {
                values.Add(new PropertyFilterValue(propertyInProject.Value.ToString()));
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
        /// Collection of all properties and their values to be used in extra filters
        /// </summary>
        public List<PropertyFilterModel> Properties { get; set; } = new List<PropertyFilterModel> ();
    }
}