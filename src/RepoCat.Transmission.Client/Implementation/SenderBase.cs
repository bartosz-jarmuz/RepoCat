// -----------------------------------------------------------------------
//  <copyright file="SenderBase.cs" company="SDL plc">
//   Copyright (c) SDL plc. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RepoCat.Transmission.Models;

namespace RepoCat.Transmission.Client
{
    public abstract class SenderBase : ISender
    {
        public virtual void SetBaseAddress(Uri baseAddress)
        {
            //not necessarily needed
        }

        /// <summary>
        /// Info level log action
        /// </summary>
        protected abstract Action<string> LogInfo { get; }
    

        public virtual async Task<RepositoryImportResult> Send(IEnumerable<ProjectInfo> infos)
        {
            if (infos == null) throw new ArgumentNullException(nameof(infos));

            List<Task<ProjectImportResult>> tasks = new List<Task<ProjectImportResult>>();
            this.LogInfo?.Invoke($"Starting sending projects RepoCat...");

            int infoCounter = 0;
            foreach (ProjectInfo projectInfo in infos)
            {
                infoCounter++;
                tasks.Add(this.Send(projectInfo));
            }
            this.LogInfo?.Invoke($"Waiting for all {infoCounter} project infos to be sent.");

            ProjectImportResult[] results = await Task.WhenAll(tasks).ConfigureAwait(false);
            this.LogInfo?.Invoke($"Finished sending all {infoCounter} project infos.");
            return new RepositoryImportResult() { ProjectResults = results };
        }

        public abstract Task<ProjectImportResult> Send(ProjectInfo info);
    }
}