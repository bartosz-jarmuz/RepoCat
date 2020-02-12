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
    public class FileListPropertyEnricher : EnricherBase
    {
        public override void Enrich(string inputUri, ProjectInfo projectInfo, string manifestFilePath)
        {
            if (projectInfo == null) return;
            
        }
    }
}