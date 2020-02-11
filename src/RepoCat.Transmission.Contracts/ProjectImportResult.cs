// -----------------------------------------------------------------------
//  <copyright file="ProjectImportResult.cs" company="bartosz.jarmuz@gmail.com">
//   Copyright (c) Bartosz Jarmuż. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using RepoCat.Transmission.Models;

namespace RepoCat.Transmission.Contracts
{
    public class ProjectImportResult
    {
        public ProjectImportResult(ProjectInfo projectInfo)
        {
            this.ProjectInfo = projectInfo;
        }

        public ProjectInfo ProjectInfo { get; set; }
        public Exception Exception { get; set; }
        public bool Success { get; set; }
        public string Response { get; set; }
    }
}