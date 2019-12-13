using AutoMapper;
using RepoCat.Persistence.Models;
using RepoCat.Portal.Areas.Catalog.Models;
using ProjectInfo = RepoCat.Persistence.Models.ProjectInfo;

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
        }

        private void MapViewModels()
        {
            this.CreateMap<Project, ProjectInfoViewModel>()
                .IncludeMembers(p=>p.ProjectInfo)
                .ForMember(x => x.RepositoryName, o=>o.MapFrom(p=>p.RepositoryInfo.RepositoryName))
                .ForMember(x => x.OrganizationName, o=>o.MapFrom(p=>p.RepositoryInfo.OrganizationName))
                
                ;
            this.CreateMap<ProjectInfo, ProjectInfoViewModel>()
                .ForMember(x => x.RepositoryName, o => o.Ignore())
                .ForMember(x=>x.OrganizationName, o=>o.Ignore())
                ;
            this.CreateMap<RepoCat.Persistence.Models.ComponentManifest, ComponentManifestViewModel>();
            this.CreateMap<ManifestQueryResult, ManifestQueryResultViewModel>()
                .ForMember(x => x.SearchTokens, o=>o.Ignore());


        }

        private void MapTransmitterModels()
        {
            this.CreateMap<Transmission.Models.ProjectInfo, ProjectInfo>()
                .ForMember(x=>x.Id, o=>o.Ignore())
                .ForMember(x=>x.RepositoryId, o=>o.Ignore())
                .ForMember(x=>x.AddedDateTime, o=>o.Ignore());
            this.CreateMap<Transmission.Models.ComponentManifest, ComponentManifest>();

        }
    }
}