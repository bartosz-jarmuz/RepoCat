// -----------------------------------------------------------------------
//  <copyright file="ProjectsTableModel.cs" company="bartosz.jarmuz@gmail.com">
//   Copyright (c) Bartosz Jarmuż. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

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
            
        }

        private void BuildPropertiesDictionary()
        {
            foreach (Dictionary<string, string> propertiesInProjects in this.Projects.Select(x => x.Properties).Concat(this.Projects.SelectMany(x=>x.Components.Select(c=>c.Properties))))
            {
                foreach (KeyValuePair<string, string> propertyInProject in propertiesInProjects)
                {
                    var propertyInViewModel = this.Properties.FirstOrDefault(p => p.Key == propertyInProject.Key);
                    if (propertyInViewModel == null)
                    {
                        var prop = new PropertyFilterModel()
                        {
                            Key = propertyInProject.Key,
                            Values = new List<PropertyFilterValue>() {new PropertyFilterValue(propertyInProject.Value)},
                            OccurenceCount = 1,
                        };

                        this.Properties.Add(prop);
                    }
                    else
                    {
                        propertyInViewModel.OccurenceCount++;
                        var existingValue =
                            propertyInViewModel.Values.FirstOrDefault(x => x.Value == propertyInProject.Value);
                        if (existingValue == null)
                        {
                            propertyInViewModel.Values.Add(new PropertyFilterValue(propertyInProject.Value));
                        }
                        else
                        {
                            existingValue.OccurenceCount++;
                        }
                    }
                }
            }

            this.Properties = this.Properties.OrderByDescending(x => x.OccurenceCount).ToList();

            foreach (PropertyFilterModel propertyModel in this.Properties)
            {
                propertyModel.Values = propertyModel.Values.OrderByDescending(x => x.OccurenceCount).ToList();
            }
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