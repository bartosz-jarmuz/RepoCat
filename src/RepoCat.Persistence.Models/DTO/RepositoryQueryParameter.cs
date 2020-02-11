// -----------------------------------------------------------------------
//  <copyright file="RepositoryQueryParameter.cs" company="bartosz.jarmuz@gmail.com">
//   Copyright (c) Bartosz Jarmuż. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

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