// -----------------------------------------------------------------------
//  <copyright file="PropertyViewModel.cs" company="bartosz.jarmuz@gmail.com">
//   Copyright (c) Bartosz Jarmuż. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace RepoCat.Portal.Areas.Catalog.Models
{
    /// <summary>
    /// Constains property plus a bit of extra contextual info
    /// </summary>
    public class PropertyPartialViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="componentName"></param>
        /// <param name="property"></param>
        public PropertyPartialViewModel(string projectId, string componentName, PropertyViewModel property)
        {
            this.ProjectId = projectId;
            this.ComponentName = componentName;
            this.Property = property;
        }

        /// <summary>
        /// 
        /// </summary>
        public string ProjectId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ComponentName { get; set; }
    
        /// <summary>
        /// 
        /// </summary>
        public PropertyViewModel Property { get; set; }
    }
}