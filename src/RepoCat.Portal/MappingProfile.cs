using System.IO;
using AutoMapper;
using RepoCat.Persistence.Models;
using RepoCat.Portal.Areas.Catalog.Models;
using RepoCat.Portal.Models;
using ProjectInfo = RepoCat.Persistence.Models.ProjectInfo;

namespace RepoCat.Portal
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
            this.CreateMap<ProjectInfo, ProjectInfoViewModel>();
            this.CreateMap<RepoCat.Persistence.Models.ComponentManifest, ComponentManifestViewModel>();
            this.CreateMap<ManifestQueryResult, ManifestQueryResultViewModel>();

        }

        private void MapTransmitterModels()
        {
            this.CreateMap<Transmission.Models.ProjectInfo, ProjectInfo>();
            this.CreateMap<Transmission.Models.ComponentManifest, ComponentManifest>();

        }
    }
}