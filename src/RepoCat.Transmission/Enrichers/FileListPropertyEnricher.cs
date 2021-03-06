﻿// -----------------------------------------------------------------------
//  <copyright file="AssemblyInfoResolvingEnricher.cs" company="bartosz.jarmuz@gmail.com">
//   Copyright (c) Bartosz Jarmuż. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.IO;
using System.Linq;
using DotNetProjectParser;
using RepoCat.Transmission.Contracts;
using RepoCat.Transmission.Models;

namespace RepoCat.Transmission
{
    public class FileListPropertyEnricher : ProjectInfoEnricherBase
    {
        public override void EnrichProjectInfo(string inputUri, ProjectInfo projectInfo, string manifestFilePath, object inputObject)
        {
            if (!(inputObject is Project project))
            {
                return;
            }
            
            projectInfo.Properties.Add(new Property("CodeFiles", project.Items
                .Where(x=>x.ItemType == "Compile"
                          || x.ItemType == "Build"
                          )
                .Select(x=>x.ItemName)));
        }
    }
}