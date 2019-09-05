using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using RepoCat.Portal.Models;

namespace RepoCat.Portal.Areas.Catalog.Models
{
    public class AddProjectModel
    {
        [Required(ErrorMessage = "Project name is required")]
        public string ProjectName { get; set; }

        public string RepositoryName { get; set; }

        public string ProjectDescription { get; set; }

        [Required(ErrorMessage = "Assembly name is required")]
        public string AssemblyName { get; set; }

        public List<ComponentManifestViewModel> Components { get; set; }

    }
}
