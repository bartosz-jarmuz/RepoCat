using AutoMapper;
using RepoCat.Models.ProjectInfo;
using RepoCat.Portal.Models;
using RepoCat.Portal.Models.Domain;

namespace RepoCat.Portal
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            this.CreateMap<ProjectInfo, ProjectManifest>();
            this.CreateMap<ProjectManifest, ProjectManifestViewModel>();
            this.CreateMap<ComponentManifest, ComponentManifestViewModel>();
        }
    }
}