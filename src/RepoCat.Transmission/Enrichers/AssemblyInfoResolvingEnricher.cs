// -----------------------------------------------------------------------
//  <copyright file="AssemblyInfoResolvingEnricher.cs" company="bartosz.jarmuz@gmail.com">
//   Copyright (c) Bartosz Jarmuż. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.IO;
using RepoCat.Transmission.Contracts;
using RepoCat.Transmission.Models;

namespace RepoCat.Transmission
{
    public class AssemblyInfoResolvingEnricher : EnricherBase
    {
        public override void Enrich(string inputUri, ProjectInfo projectInfo, string manifestFilePath)
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