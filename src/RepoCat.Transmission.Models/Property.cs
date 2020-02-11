﻿// -----------------------------------------------------------------------
//  <copyright file="Property.cs" company="bartosz.jarmuz@gmail.com">
//   Copyright (c) Bartosz Jarmuż. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace RepoCat.Transmission.Models
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
        public Property(string key, object value)
        {
            this.Key = key;
            this.Value = value;
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
        public object Value { get; set; }
    }
}