using AutoMapper;
using RepoCat.Models.ProjectInfo;
using RepoCat.Persistence.Models;
using RepoCat.Persistence.Service;
using RepoCat.Portal.Models;

namespace RepoCat.Portal
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            this.CreateMap<ProjectInfo, ProjectManifest>();
            this.CreateMap<ProjectManifest, ProjectManifestViewModel>();
            this.CreateMap<ComponentManifest, ComponentManifestViewModel>();
            this.CreateMap<ManifestQueryResult, ManifestQueryResultViewModel>();
        }
    }
}