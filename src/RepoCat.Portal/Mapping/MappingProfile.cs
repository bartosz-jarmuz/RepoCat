// -----------------------------------------------------------------------
//  <copyright file="MappingProfile.cs" company="bartosz.jarmuz@gmail.com">
//   Copyright (c) Bartosz Jarmuż. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using AutoMapper;
using RepoCat.Persistence.Models;
using RepoCat.Portal.Areas.Catalog.Models;
using RepoCat.Portal.RecurringJobs;
using RepoCat.RepositoryManagement.Service;
using RepoCat.Transmission.Contracts;
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
                .ForMember(x => x.DisplayRepositoryName, o => o.Ignore())
                ;
            ;
            this.CreateMap<ProjectInfo, ProjectInfoViewModel>()
                .ForMember(x => x.RepositoryName, o => o.Ignore())
                .ForMember(x => x.DisplayRepositoryName, o => o.Ignore())
                .ForMember(x => x.OrganizationName, o => o.Ignore())
                .ForMember(x => x.Properties, o => o.Ignore())
                .AfterMap((src, dest, rc) =>
                {
                    foreach (Property prop in src.Properties) dest.Properties.Add(prop.Key, prop.Value);
                })
                ;

            this.CreateMap<ComponentManifest, ComponentManifestViewModel>()
                .ForMember(x => x.Properties, o => o.Ignore())
                .AfterMap((src, dest, rc) =>
                {
                    foreach (Property prop in src.Properties) dest.Properties.Add(prop.Key, prop.Value);
                })
                ;
            this.CreateMap<ManifestQueryResult, ManifestQueryResultViewModel>()
                .ForMember(x => x.SearchTokens, o => o.Ignore())
                .ForMember(x => x.ProjectsTable, o => o.Ignore())
                ;
            this.CreateMap<CollectionSummary, Areas.Admin.Models.CollectionSummary>();
        }

        private void MapPersistenceModels()
        {
            this.CreateMap<RepositoryQueryParameter, Persistence.Models.RepositoryQueryParameter>();

            this.CreateMap<SearchStatistics, RepositoryManagement.Service.SearchStatistics>();
            this.CreateMap<SearchKeywordData, RepositoryManagement.Service.SearchKeywordData>();
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