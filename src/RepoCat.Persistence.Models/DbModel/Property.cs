// -----------------------------------------------------------------------
//  <copyright file="Property.cs" company="bartosz.jarmuz@gmail.com">
//   Copyright (c) Bartosz Jarmuż. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;

namespace RepoCat.Persistence.Models
{
    /// <summary>
    /// Holds a single property data
    /// </summary>
    public class Property
    {
        /// <summary>
        /// New instance
        /// </summary>
        public Property()
        {
        }

        /// <summary>
        /// New instance
        /// </summary>
        public Property(string name, object value)
        {
            this.Key = name;

            if (value != null)
            {
                if (value.GetType() != typeof(string) && value is IEnumerable enumerable)
                {
                    foreach (object o in enumerable)
                    {
                        this.ValueList.Add(o?.ToString());
                    }
                }
                else
                {
                    this.Value = value?.ToString();
                }
            }
        }

        /// <summary>
        /// Create from key value pair
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator Property((string key, object value) value)
        {
            return new Property(value.key, value.value);
        }
        /// <summary>
        /// Name of the property
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Value of the property
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<string> ValueList { get; set; } = new List<string>();
    }
}