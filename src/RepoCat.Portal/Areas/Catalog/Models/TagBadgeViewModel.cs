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
    public class TagBadgeViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TagBadgeViewModel"/> class.
        /// </summary>
        /// <param name="repositoryQueryParameters"></param>
        /// <param name="tagText">The tag text.</param>
        public TagBadgeViewModel(IReadOnlyCollection<RepositoryQueryParameter> repositoryQueryParameters, string tagText)
        {
            this.RepositoryQueryParameters = repositoryQueryParameters;
            this.TagText = tagText;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="organizationName"></param>
        /// <param name="repositoryName"></param>
        /// <param name="tagText"></param>
        public TagBadgeViewModel(string organizationName, string repositoryName, string tagText)
        {
            this.RepositoryQueryParameters = new []{new RepositoryQueryParameter(organizationName, repositoryName), };
            this.TagText = tagText;
        }

        /// <summary>
        /// 
        /// </summary>
        public IReadOnlyCollection<RepositoryQueryParameter> RepositoryQueryParameters { get; }

        /// <summary>
        /// Gets or sets the tag text.
        /// </summary>
        /// <value>The tag text.</value>
        public string TagText { get; set; }
    }
}
