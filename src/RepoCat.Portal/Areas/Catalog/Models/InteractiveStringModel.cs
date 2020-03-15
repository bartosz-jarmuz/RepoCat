// -----------------------------------------------------------------------
//  <copyright file="InteractiveStringModel.cs" company="bartosz.jarmuz@gmail.com">
//   Copyright (c) Bartosz Jarmuż. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Collections;
using System.Linq;
using System.Text.RegularExpressions;

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
        public object PropertyValue { get; set; }

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
        /// <param name="propertyValue"></param>
        /// <param name="className"></param>
        /// <param name="imageClassName"></param>
        public InteractiveStringModel(string projectId, string componentName, string propertyKey, object propertyValue, string className = "", string imageClassName = "")
        {
            this.ProjectId = projectId;
            this.ComponentName = componentName;
            this.PropertyKey = propertyKey;
            this.SetPropertyValue(propertyValue);
            this.ImageClassName = imageClassName;
            this.ClassName = className;
        }

        private void SetPropertyValue(object propertyValue)
        {
            if (propertyValue is PropertyViewModel vm)
            {
                if (vm.ValueList != null && vm.ValueList.Any())
                {
                    this.PropertyValue = vm.ValueList;
                }
                else
                {
                    this.PropertyValue = vm.Value;
                }
            }
            else
            {
                this.PropertyValue = propertyValue;
            }
        }


        private bool IsEmail()
        {
            if (Regex.IsMatch(this.PropertyValue.ToString(), @"^\S+@\S+\.\S+$"))//simplified but not as huge as it could be
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
            if (string.IsNullOrEmpty(this.PropertyValue?.ToString()))
            {
                return DataType.Text;
            }

            if (this.PropertyValue.GetType() != typeof(string) && this.PropertyValue is IEnumerable)
            {
                return DataType.Collection;
            }

            if (this.PropertyValue.ToString().StartsWith("http") || this.PropertyValue.ToString().StartsWith("www."))
            {
                return DataType.Url;
            }

            if (this.PropertyValue.ToString().StartsWith("\\\\"))
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
            /// <summary>
            /// Render a collection control
            /// </summary>
            Collection
        }
    }
}
