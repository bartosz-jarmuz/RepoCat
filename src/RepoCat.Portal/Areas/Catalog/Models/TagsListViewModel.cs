// -----------------------------------------------------------------------
//  <copyright file="TagBadgeViewModel.cs" company="bartosz.jarmuz@gmail.com">
//   Copyright (c) Bartosz Jarmuż. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using RepoCat.RepositoryManagement.Service;

namespace RepoCat.Portal.Areas.Catalog.Models
{
    /// <summary>
    /// Class TagBadgeViewModel.
    /// </summary>
    public class TagsListViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string OrganizationName { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string RepositoryName { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public List<string> Tags { get; set; } = new List<string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="TagBadgeViewModel"/> class.
        /// </summary>
        public TagsListViewModel(string organizationName, string repositoryName, params IEnumerable<string>[] tagsLists)
        {
            this.OrganizationName = organizationName;
            this.RepositoryName = repositoryName;
            foreach (IEnumerable<string> tags in tagsLists)
            {
               this.Tags.AddRange(tags);
            }
        }

        
    }
}
