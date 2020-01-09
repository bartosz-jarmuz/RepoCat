using AutoMapper;
using RepoCat.Persistence.Models;
using RepoCat.Portal.Areas.Catalog.Models;
using RepoCat.Portal.RecurringJobs;
using RepoCat.RepositoryManagement.Service;
using RepoCat.Transmission;
using ProjectInfo = RepoCat.Persistence.Models.ProjectInfo;
using RepositoryQueryParameter = RepoCat.Persistence.Models.RepositoryQueryParameter;

namespace RepoCat.Portal.Mapping
{
    /// <summary>
    /// Class MappingProfile.
    /// </summary>
    /// <seealso cref="Profile" />
    public class MappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MappingProfile"/> class.
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
                .IncludeMembers(p=>p.ProjectInfo)
                .ForMember(x => x.RepositoryName, o=>o.MapFrom(p=>p.RepositoryInfo.RepositoryName))
                .ForMember(x => x.OrganizationName, o=>o.MapFrom(p=>p.RepositoryInfo.OrganizationName))
                .ForMember(x => x.DisplayRepositoryName, o=>o.Ignore())
                ;
            this.CreateMap<ProjectInfo, ProjectInfoViewModel>()
                .ForMember(x => x.RepositoryName, o => o.Ignore())
                .ForMember(x => x.DisplayRepositoryName, o=>o.Ignore())
                .ForMember(x=>x.OrganizationName, o=>o.Ignore())
                ;
            this.CreateMap<RepoCat.Persistence.Models.ComponentManifest, ComponentManifestViewModel>();
            this.CreateMap<ManifestQueryResult, ManifestQueryResultViewModel>().ForMember(x => x.SearchTokens, o=>o.Ignore());
        }

        private void MapPersistenceModels()
        {
            this.CreateMap<RepoCat.RepositoryManagement.Service.RepositoryQueryParameter, RepositoryQueryParameter>();
        }


        private void MapTransmitterModels()
        {
            this.CreateMap<Transmission.Models.ProjectInfo, ProjectInfo>()
                .ForMember(x=>x.Id, o=>o.Ignore())
                .ForMember(x=>x.RepositoryId, o=>o.Ignore())
                .ForMember(x=>x.AddedDateTime, o=>o.Ignore());
            this.CreateMap<Transmission.Models.ComponentManifest, ComponentManifest>();
            this.CreateMap<Transmission.Models.RepositoryInfo, RepositoryInfo>()
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