using System.Collections.Generic;

namespace RepoCat.Portal.Areas.Catalog.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class PropertyViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="componentName"></param>
        /// <param name="property"></param>
        public PropertyViewModel(string projectId, string componentName, KeyValuePair<string, string> property)
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
        public KeyValuePair<string, string> Property { get; set; }
    }
}