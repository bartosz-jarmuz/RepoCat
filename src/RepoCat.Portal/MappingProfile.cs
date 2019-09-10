using AutoMapper;
using RepoCat.Models.Manifests;
using RepoCat.Models.ProjectInfo;
using RepoCat.Portal.Models;
using RepoCat.Web.Persistence;

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