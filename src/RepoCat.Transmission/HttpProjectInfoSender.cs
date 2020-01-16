using System;
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


        protected override Action<string> LogInfo => (s) => this.logger.Info(s);

        /// <summary>
        /// Sends the specified information.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <returns>Task.</returns>
        public override async Task<ProjectImportResult> Send(ProjectInfo info)
        {
            if (info == null) throw new ArgumentNullException(nameof(info));
            var projectImportResult = new ProjectImportResult(info);
            this.logger.Debug($"Sending {info.ProjectName} project info");

            string serialized;
            try
            {
                serialized = JsonConvert.SerializeObject(info);
            }   
            catch (Exception ex)
            {
                this.logger.Error($"Error while serializing project info: {info.ProjectName}", ex);
                projectImportResult.Exception = ex;
                return projectImportResult;
            }
            
            try
            {
                using (StringContent content = new StringContent(serialized, Encoding.UTF8, "application/json"))
                {
                    HttpResponseMessage result = await this.client.PostAsync("api/manifest", content).ConfigureAwait(false);
                    if (result.IsSuccessStatusCode)
                    {
                        this.logger.Info(
                            $"Sent [{info.ProjectName}]. StatusCode: [{result.StatusCode}]. Location: [{result.Headers?.Location}]");
                        string response = await result.Content.ReadAsStringAsync().ConfigureAwait(false);
                        this.logger.Debug($"Response [{response}].");
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

                        this.logger.Error(
                            $"Error - [{result.StatusCode}] - [{result.ReasonPhrase}] - while sending [{info.ProjectName}]. {response}");
                        projectImportResult.Exception = new InvalidOperationException($"Error - [{result.StatusCode}] - [{result.ReasonPhrase}] - while sending [{info.ProjectName}]. {response}");
                        projectImportResult.Response = response;
                        return projectImportResult;
                    }
                }
            }
            catch (Exception ex)
            {
                this.logger.Error($"Unexpected error while sending project info: {info.ProjectName}. {serialized}", ex);
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