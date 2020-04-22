// -----------------------------------------------------------------------
//  <copyright file="MappingProfile.cs" company="bartosz.jarmuz@gmail.com">
//   Copyright (c) Bartosz Jarmuż. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using AutoMapper;
using RepoCat.Persistence.Models;
using RepoCat.Portal.Areas.Catalog.Models;
using RepoCat.Portal.RecurringJobs;
using RepoCat.RepositoryManagement.Service;
using RepoCat.Transmission.Contracts;
using DownloadStatistics = RepoCat.Persistence.Models.DownloadStatistics;
using ProjectDownloadData = RepoCat.Persistence.Models.ProjectDownloadData;
using RepositoryInfo = RepoCat.Transmission.Models.RepositoryInfo;
using RepositoryQueryParameter = RepoCat.RepositoryManagement.Service.RepositoryQueryParameter;
using SearchKeywordData = RepoCat.Persistence.Models.SearchKeywordData;
using SearchStatistics = RepoCat.Persistence.Models.SearchStatistics;

namespace RepoCat.Portal.Mapping
{
    /// <summary>
    ///     Class MappingProfile.
    /// </summary>
    /// <seealso cref="Profile" />
    public class MappingProfile : Profile
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="MappingProfile" /> class.
        /// </summary>
        public MappingProfile()
        {
            this.MapTransmitterModels();
            this.MapViewModels();
            this.MapPersistenceModels();
        }

        private void MapViewModels()
        {
            this.CreateMap<Project, ProjectInfoViewModel>()
                .IncludeMembers(p => p.ProjectInfo)
                .ForMember(x => x.RepositoryName, o => o.MapFrom(p => p.RepositoryInfo.RepositoryName))
                .ForMember(x => x.OrganizationName, o => o.MapFrom(p => p.RepositoryInfo.OrganizationName))
                .ForMember(x => x.SearchAccuracyScore, o => o.MapFrom(p => p.SearchAccuracyScore))
                .ForMember(x => x.DisplayRepositoryName, o => o.Ignore())
                .ForMember(x => x.Properties, o => o.Ignore())
                ;
            

            this.CreateMap<ProjectInfo, ProjectInfoViewModel>()
                .ForMember(x => x.RepositoryName, o => o.Ignore())
                .ForMember(x => x.DisplayRepositoryName, o => o.Ignore())
                .ForMember(x => x.OrganizationName, o => o.Ignore())
                .ForMember(x => x.Properties, o => o.Ignore())
                .ForMember(x => x.SearchAccuracyScore, o => o.Ignore())
                
                .AfterMap((src, dest, rc) =>
                {
                    foreach (Property prop in src.Properties) dest.Properties.Add(new PropertyViewModel(){Key = prop.Key, Value = prop.Value, ValueList = prop.ValueList});
                })
                ;

            this.CreateMap<ComponentManifest, ComponentManifestViewModel>()
                .ForMember(x => x.Properties, o => o.Ignore())
                .AfterMap((src, dest, rc) =>
                {
                    foreach (Property prop in src.Properties) dest.Properties.Add(new PropertyViewModel(){Key = prop.Key, Value = prop.Value, ValueList = prop.ValueList});
                })
                ;
            this.CreateMap<ManifestQueryResult, ManifestQueryResultViewModel>()

                .ForMember(x => x.ProjectsTable, o => o.Ignore())
                ;


            this.CreateMap<CollectionSummary, Areas.Admin.Models.CollectionSummary>();
        }

        private void MapPersistenceModels()
        {
            this.CreateMap<RepositoryQueryParameter, Persistence.Models.RepositoryQueryParameter>();

            this.CreateMap<SearchStatistics, RepositoryManagement.Service.SearchStatistics>();
            this.CreateMap<SearchKeywordData, RepositoryManagement.Service.SearchKeywordData>();

            this.CreateMap<DownloadStatistics, RepositoryManagement.Service.DownloadStatistics>();
            this.CreateMap<ProjectDownloadData, RepositoryManagement.Service.ProjectDownloadData>();

        }

        private void MapTransmitterModels()
        {
            this.CreateMap<Transmission.Models.Property, Property>();

            this.CreateMap<Transmission.Models.ProjectInfo, ProjectInfo>()
                .ForMember(x => x.Id, o => o.Ignore())
                .ForMember(x => x.RepositoryId, o => o.Ignore())
                .ForMember(x => x.AddedDateTime, o => o.Ignore())
                .ForMember(dest => dest.Properties, opt => opt.Ignore())
                .AfterMap((src, dest, rc) =>
                {
                    foreach (Transmission.Models.Property prop in src.Properties)
                        dest.Properties.Add(rc.Mapper.Map<Property>(prop));
                });

            this.CreateMap<Transmission.Models.ComponentManifest, ComponentManifest>()
                .ForMember(dest => dest.Properties, opt => opt.Ignore())
                .AfterMap((src, dest, rc) =>
                {
                    foreach (Transmission.Models.Property prop in src.Properties)
                        dest.Properties.Add(rc.Mapper.Map<Property>(prop));
                })
                ;


            this.CreateMap<RepositoryInfo, Persistence.Models.RepositoryInfo>()
                .ForMember(x => x.Id, o => o.Ignore())
                ;
            this.CreateMap<RepositoryToScanSettings, TransmitterArguments>()
                .ForMember(x => x.CodeRootFolder, o => o.MapFrom(s => s.RepositoryPath))
                .ForMember(x => x.ApiBaseUri, o => o.Ignore())
                .ForMember(x => x.ProjectPaths, o => o.Ignore())
                .ForMember(x => x.OriginalParameterInputString, o => o.Ignore())
                .ForMember(x => x.OriginalParameterCollection, o => o.Ignore())
                .ForMember(x => x.RepositoryStamp, o => o.Ignore())
                ;
        }
    }
}