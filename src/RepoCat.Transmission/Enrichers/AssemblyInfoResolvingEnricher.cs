using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using RepoCat.Transmission.Contracts;
using RepoCat.Transmission.Models;

namespace RepoCat.Transmission
{
    public class AssemblyInfoResolvingEnricher : EnricherBase
    {
        public override void Enrich(ProjectInfo projectInfo, string manifestFilePath, string inputUri)
        {
            if (projectInfo == null) return;
            if (string.IsNullOrEmpty(projectInfo.DownloadLocation)) return;

            //we might get some info based on where the actual executable is
            
            if (string.IsNullOrEmpty(projectInfo.AssemblyName))
            {
                projectInfo.AssemblyName = Path.GetFileName(projectInfo.DownloadLocation);
            }

            if (string.IsNullOrEmpty(projectInfo.TargetExtension))
            {
                projectInfo.TargetExtension = Path.GetExtension(projectInfo.DownloadLocation);
            }

            if (string.IsNullOrEmpty(projectInfo.OutputType))
            {
                projectInfo.OutputType = projectInfo.TargetExtension;
            }





        }
    }
}