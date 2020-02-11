// -----------------------------------------------------------------------
//  <copyright file="IProjectInfoBuilder.cs" company="bartosz.jarmuz@gmail.com">
//   Copyright (c) Bartosz Jarmuż. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using RepoCat.Transmission.Models;

namespace RepoCat.Transmission.Contracts
{
    public interface IProjectInfoBuilder
    {
        IList<IProjectInfoEnricher> ProjectInfoEnrichers { get; }
        IEnumerable<ProjectInfo> GetInfos(IEnumerable<string> uris);
    }
}