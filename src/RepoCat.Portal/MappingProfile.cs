using AutoMapper;
using RepoCat.Persistence.Models;
using RepoCat.Portal.Models;
using ProjectInfo = RepoCat.Persistence.Models.ProjectInfo;

namespace RepoCat.Portal
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            this.CreateMap<Transmitter.Models.ProjectInfo, ProjectInfo>();
            this.CreateMap<ProjectInfo, ProjectManifestViewModel>();
            this.CreateMap<RepoCat.Persistence.Models.ComponentManifest, ComponentManifestViewModel>();
            this.CreateMap<ManifestQueryResult, ManifestQueryResultViewModel>();
        }
    }
}