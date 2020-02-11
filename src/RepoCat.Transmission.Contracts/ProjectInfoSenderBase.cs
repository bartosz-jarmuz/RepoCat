// -----------------------------------------------------------------------
//  <copyright file="ProjectInfoSenderBase.cs" company="bartosz.jarmuz@gmail.com">
//   Copyright (c) Bartosz Jarmuż. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RepoCat.Transmission.Models;

namespace RepoCat.Transmission.Contracts
{
    public abstract class ProjectInfoSenderBase : IProjectInfoSender
    {
        public virtual void SetBaseAddress(Uri baseAddress)
        {
            //not necessarily needed
        }

        /// <summary>
        /// 
        /// </summary>
        protected abstract IProgress<ProjectImportProgressData> ProgressLog { get;  }



        public virtual async Task<RepositoryImportResult> Send(IEnumerable<ProjectInfo> infos)
        {
            if (infos == null) throw new ArgumentNullException(nameof(infos));

            List<Task<ProjectImportResult>> tasks = new List<Task<ProjectImportResult>>();
            this.ProgressLog.Report(new ProjectImportProgressData(ProjectImportProgressData.VerbosityLevel.Info,$"Starting sending projects RepoCat..."));
            int infoCounter = 0;

            foreach (ProjectInfo projectInfo in infos)
            {
                infoCounter++;
                tasks.Add(this.Send(projectInfo));
            }
            this.ProgressLog.Report(new ProjectImportProgressData(ProjectImportProgressData.VerbosityLevel.Info, $"Waiting for all {infoCounter} project infos to be sent."));

            ProjectImportResult[] results = await Task.WhenAll(tasks).ConfigureAwait(false);
            this.ProgressLog.Report(new ProjectImportProgressData(ProjectImportProgressData.VerbosityLevel.Info, $"Finished sending all {infoCounter} project infos."));
            return new RepositoryImportResult() { ProjectResults = results };
        }

        public abstract Task<ProjectImportResult> Send(ProjectInfo info);
    }


    public class ProjectImportProgressData
    {
        public ProjectImportProgressData(string message, Exception exception)
        {
            this.Verbosity = VerbosityLevel.Error;
            this.Message = message;
            this.Exception = exception;
        }

        public ProjectImportProgressData(VerbosityLevel verbosity, string message)
        {
            this.Verbosity = verbosity;
            this.Message = message;
        }

        public enum VerbosityLevel
        {
            Info,
            Debug,
            Error,
            Warn
        }

        public VerbosityLevel Verbosity { get; set; }

        public string Message { get; set; }

        public Exception Exception { get; set; }
    }
}