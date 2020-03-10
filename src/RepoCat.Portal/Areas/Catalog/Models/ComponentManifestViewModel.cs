// -----------------------------------------------------------------------
//  <copyright file="ComponentManifestViewModel.cs" company="bartosz.jarmuz@gmail.com">
//   Copyright (c) Bartosz Jarmuż. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;

namespace RepoCat.Portal.Areas.Catalog.Models
{
    /// <summary>
    /// Class ComponentManifestViewModel.
    /// </summary>
    public class ComponentManifestViewModel
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description { get; set; }
        /// <summary>
        /// Gets or sets the documentation URI.
        /// </summary>
        /// <value>The documentation URI.</value>
        public string DocumentationUri { get; set; }
        /// <summary>
        /// Gets or sets the tags.
        /// </summary>
        /// <value>The tags.</value>
        public List<string> Tags { get; internal set; } = new List<string>();
        /// <summary>
        /// Gets or sets the properties.
        /// </summary>
        /// <value>The properties.</value>
        public Dictionary<string, object> Properties { get; internal set; } = new Dictionary<string, object>();
    }
}