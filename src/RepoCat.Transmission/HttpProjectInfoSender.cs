// -----------------------------------------------------------------------
//  <copyright file="HttpProjectInfoSender.cs" company="bartosz.jarmuz@gmail.com">
//   Copyright (c) Bartosz Jarmuż. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RepoCat.Transmission.Contracts;
using RepoCat.Transmission.Models;

namespace RepoCat.Transmission
{
    /// <summary>
    /// Class that sends the project manifests to the RepoCat API over HTTP
    /// </summary>
    public class HttpProjectInfoSender : ProjectInfoSenderBase, IProjectInfoSender, IDisposable
    {
        /// <summary>
        /// The client
        /// </summary>
        private readonly HttpClient client;

        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpProjectInfoSender"/> class.
        /// </summary>
        /// <param name="logger"></param>
        public HttpProjectInfoSender(ILogger logger)
        {
            this.logger = logger;
            this.client = new HttpClient();
        }

        public override void SetBaseAddress(Uri baseAddress)
        {
            this.client.BaseAddress = baseAddress;
        }

        /// <summary>
        /// Sends messages to logger
        /// </summary>
        protected override IProgress<ProjectImportProgressData> ProgressLog=>new Progress<ProjectImportProgressData>(
            data =>
            {
                switch (data.Verbosity)
                {
                    case ProjectImportProgressData.VerbosityLevel.Info:
                        this.logger.Info(data.Message);
                        break;
                    case ProjectImportProgressData.VerbosityLevel.Debug:
                        this.logger.Debug(data.Message);
                        break;
                    case ProjectImportProgressData.VerbosityLevel.Error:
                        this.logger.Error(data.Message, data.Exception);
                        break;
                    case ProjectImportProgressData.VerbosityLevel.Warn:
                        this.logger.Warn(data.Message);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            });


        /// <summary>
        /// Sends the specified information.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <returns>Task.</returns>
        public override async Task<ProjectImportResult> Send(ProjectInfo info)
        {
            if (info == null) throw new ArgumentNullException(nameof(info));
            var projectImportResult = new ProjectImportResult(info);
            this.ProgressLog.Report(new ProjectImportProgressData(ProjectImportProgressData.VerbosityLevel.Debug, $"Serializing {info.ProjectName} project info"));

            string serialized;
            try
            {
                serialized = JsonConvert.SerializeObject(info);
            }   
            catch (Exception ex)
            {
                this.ProgressLog.Report(new ProjectImportProgressData($"Error while serializing project info: {info.ProjectName}", ex));
                projectImportResult.Exception = ex;
                return projectImportResult;
            }
            this.ProgressLog.Report(new ProjectImportProgressData(ProjectImportProgressData.VerbosityLevel.Info, $"Sending {info.ProjectName} project info"));
            Stopwatch sw = Stopwatch.StartNew();
            try
            {
                using (StringContent content = new StringContent(serialized, Encoding.UTF8, "application/json"))
                {
                    HttpResponseMessage result = await this.client.PostAsync("api/manifest", content).ConfigureAwait(false);
                    sw.Stop();
                    if (result.IsSuccessStatusCode)
                    {
                        this.ProgressLog.Report(new ProjectImportProgressData(ProjectImportProgressData.VerbosityLevel.Info,
                          $"Sent [{info.ProjectName}]. StatusCode: [{result.StatusCode}]. Elapsed: [{sw.ElapsedMilliseconds}ms] Response: [{result.Headers?.Location}]"));
                        string response = await result.Content.ReadAsStringAsync().ConfigureAwait(false);
                        this.ProgressLog.Report(new ProjectImportProgressData(ProjectImportProgressData.VerbosityLevel.Debug, $"Response [{response}]."));
                        projectImportResult.Success = true;
                        projectImportResult.Response = $"Location: [{result.Headers?.Location}]";
                        return projectImportResult;
                    }
                    else
                    {
                        string response;
                        try
                        {
                            response = await result.Content.ReadAsStringAsync().ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            response = "Error while reading http response content: " + ex;
                        }

                        this.ProgressLog.Report(new ProjectImportProgressData(ProjectImportProgressData.VerbosityLevel.Error,
                          $"Error - [{result.StatusCode}] - [{result.ReasonPhrase}] - while sending [{info.ProjectName}]. Elapsed: [{sw.ElapsedMilliseconds}ms]. {response}"));
                        projectImportResult.Exception = new InvalidOperationException($"Error - [{result.StatusCode}] - [{result.ReasonPhrase}] - while sending [{info.ProjectName}]. {response}");
                        projectImportResult.Response = response;
                        return projectImportResult;
                    }
                }
            }
            catch (Exception ex)
            {
                sw.Stop();
                this.ProgressLog.Report(new ProjectImportProgressData($"Unexpected error while sending project info: {info.ProjectName}. Elapsed: [{sw.ElapsedMilliseconds}ms]. {serialized}", ex));
                projectImportResult.Exception = ex;
                return projectImportResult;
            }

        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);

        }

        protected virtual void Dispose(bool disposing)
        {
             this.client?.Dispose();
        }
    }
}