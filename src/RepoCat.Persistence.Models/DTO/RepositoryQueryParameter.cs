// -----------------------------------------------------------------------
//  <copyright file="RepositoryQueryParameter.cs" company="SDL plc">
//   Copyright (c) SDL plc. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace RepoCat.Persistence.Models
{
    /// <summary>
    /// A parameter object for repository queries<br/>
    /// Stores data about repository and organization
    /// </summary>
    public class RepositoryQueryParameter
    {
        /// <summary>
        /// 
        /// </summary>
        public string OrganizationName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string RepositoryName { get; set; }
    }
}