﻿// -----------------------------------------------------------------------
//  <copyright file="MappingConfigurationFactory.cs" company="bartosz.jarmuz@gmail.com">
//   Copyright (c) Bartosz Jarmuż. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using AutoMapper;

namespace RepoCat.Portal.Mapping
{
    /// <summary>
    /// Generates the AutoMapper configurations
    /// </summary>
    public static class MappingConfigurationFactory
    {
        /// <summary>
        /// Create all the needed configs
        /// </summary>
        /// <returns></returns>
        public static MapperConfiguration Create()
        {
            var mappingConfig = new MapperConfiguration(mc => { mc.AddProfile(new MappingProfile()); });
            return mappingConfig;

        }
    }
}