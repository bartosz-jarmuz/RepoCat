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
        /// Limits the number of initially visible tags
        /// </summary>
        public int TagsInitialDisplayLimit { get; }

        /// <summary>
        /// 
        /// </summary>
        public List<string> Tags { get; set; } = new List<string>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="organizationName"></param>
        /// <param name="repositoryName"></param>
        /// <param name="tagsInitialDisplayLimit">Pass null to use default value. Pass -1 to set 'unlimited'</param>
        /// <param name="tagsLists"></param>
        public TagsListViewModel(string organizationName, string repositoryName, int? tagsInitialDisplayLimit, params IEnumerable<string>[] tagsLists)
        {
            this.OrganizationName = organizationName;
            this.RepositoryName = repositoryName;
            this.TagsInitialDisplayLimit = tagsInitialDisplayLimit??4;
            if (this.TagsInitialDisplayLimit == -1)
            {
                this.TagsInitialDisplayLimit = 10000; //expect more that 10k tags...?
            }
            foreach (IEnumerable<string> tags in tagsLists)
            {
               this.Tags.AddRange(tags);
            }
        }

        
    }
}
