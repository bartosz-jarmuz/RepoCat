// -----------------------------------------------------------------------
//  <copyright file="PropertiesCollection.cs" company="bartosz.jarmuz@gmail.com">
//   Copyright (c) Bartosz Jarmuż. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Collections.ObjectModel;
using System.Linq;

namespace RepoCat.Persistence.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class PropertiesCollection : KeyedCollection<string, Property>
    {
        /// <summary>
        /// 
        /// </summary>
        public PropertiesCollection()
        {
        }

        /// <summary>
        /// Create from key value pair
        /// </summary>
        /// <param name="properties"></param>
        public static implicit operator PropertiesCollection((string key, object value)[] properties)
        {
            var props = new PropertiesCollection();
            foreach (Property property in properties)
            {
                props.Add(property);
            }
            return props;
        }
        /// <summary>
        /// Gets the key
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected override string GetKeyForItem(Property item)
        {
            return item.Key;
        }

        /// <summary>
        /// Adds a new item
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void Add(string name, object value)
        {
            this.Items.Add(new Property(name, value));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public new object this[string key] => this.Items.First(x => x.Key == key).Value;
    }
}