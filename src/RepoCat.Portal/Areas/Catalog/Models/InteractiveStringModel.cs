using System.Collections.Generic;
using System.Text.RegularExpressions;
using RepoCat.RepositoryManagement.Service;

namespace RepoCat.Portal.Areas.Catalog.Models
{
    /// <summary>
    /// Class TagBadgeViewModel.
    /// </summary>
    public class InteractiveStringModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string ImageClassName { get; }

        /// <summary>
        /// 
        /// </summary>
        public string ProjectId { get; }
        /// <summary>
        /// 
        /// </summary>
        public string ComponentName { get; }
        
        /// <summary>
        /// 
        /// </summary>
        public string PropertyKey { get; }

        /// <summary>
        /// 
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ClassName { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="componentName"></param>
        /// <param name="propertyKey"></param>
        /// <param name="text"></param>
        /// <param name="className"></param>
        /// <param name="imageClassName"></param>
        public InteractiveStringModel(string projectId, string componentName, string propertyKey, string text, string className = "", string imageClassName = "")
        {
            this.ProjectId = projectId;
            this.ComponentName = componentName;
            this.PropertyKey = propertyKey;
            this.Text = text;
            this.ImageClassName = imageClassName;
            this.ClassName = className;
        }


        private bool IsEmail()
        {
            if (Regex.IsMatch(this.Text, @"^\S+@\S+\.\S+$"))//simplified but not as huge as it could be
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets the type of data represented by this string
        /// </summary>
        public DataType GetDataType()
        {
            if (string.IsNullOrEmpty(this.Text))
            {
                return DataType.Text;
            }

            if (this.Text.StartsWith("http") || this.Text.StartsWith("www."))
            {
                return DataType.Url;
            }

            if (this.Text.StartsWith("\\\\"))
            {
                return DataType.Path;
            }

            if (this.IsEmail())
            {
                return DataType.Email;
            }
            return DataType.Text;
        }

        /// <summary>
        /// Type of string 
        /// </summary>
        public enum DataType
        {
            /// <summary>
            /// Default rendering mode
            /// </summary>
            Text,
            /// <summary>
            /// Render an anchor tag 
            /// </summary>
            Url,
            /// <summary>
            /// Render a download action
            /// </summary>
            Path,
            /// <summary>
            /// Render a mailto tag
            /// </summary>
            Email,
        }
    }
}
