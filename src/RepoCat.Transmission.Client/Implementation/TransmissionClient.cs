using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DotNetLittleHelpers;
using RepoCat.Transmission.Models;

namespace RepoCat.Transmission.Client
{
    /// <summary>
    /// Main worker class
    /// </summary>
    public class TransmissionClient : ITransmissionClient
    {
        private readonly ILogger logger;
        private readonly ISender sender;

        /// <summary>
        /// Creates new instance
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="sender">The component that performs actual delivery of generated project info to the API (via HTTP or directly to DB)</param>
        public TransmissionClient(ILogger logger, ISender sender)
        {
            this.logger = logger;
            this.sender = sender;
        }

        ///<inheritdoc cref="ITransmissionClient"/>
        public Task Work(string[] args)
        {
            TransmitterArguments arguments= new TransmitterArguments(args);
            this.logger.Debug($"Arguments: {arguments.OriginalParameterInputString}");
            return this.Work(arguments);
        }

        ///<inheritdoc cref="ITransmissionClient"/>

        public IList<IProjectInfoEnricher> AdditionalProjectInfoEnrichers { get; } = new List<IProjectInfoEnricher>();

    ///<inheritdoc cref="ITransmissionClient"/>
        public async Task<RepositoryImportResult> Work(TransmitterArguments args, IInputUriProvider uriProvider = null, IProjectInfoBuilder projectInfoBuilder = null)
        {
            if (args == null) throw new ArgumentNullException(nameof(args));

            try
            {
                this.DisplayParameters(args);

                IEnumerable<string> uris = this.GetPaths(args, uriProvider?? UriProviderFactory.Get(args));

                if (projectInfoBuilder == null)
                {
                    projectInfoBuilder = ProjectInfoBuilderFactory.Get(args, this.logger);
                }

                this.AddEnrichersToBuilder(projectInfoBuilder);

                IEnumerable<ProjectInfo> infos = projectInfoBuilder.GetInfos(uris);

                this.sender.SetBaseAddress(args.ApiBaseUri);

                var result =  await this.sender.Send(infos).ConfigureAwait(false);

                this.logger.Info("All done");
                return result;
            }
            catch (Exception ex)
            {
                this.logger.Fatal(ex);
                throw;
            }
        }

    private void AddEnrichersToBuilder(IProjectInfoBuilder projectInfoBuilder)
    {
        foreach (IProjectInfoEnricher additionalProjectInfoEnricher in this.AdditionalProjectInfoEnrichers)
        {
            this.logger.Debug(
                $"Adding {additionalProjectInfoEnricher.GetType().Name} to {projectInfoBuilder.GetType().Name}");
            projectInfoBuilder.ProjectInfoEnrichers.Add(additionalProjectInfoEnricher);
        }

        this.logger.Info($"Enrichers added to {projectInfoBuilder.GetType().Name}: " +
                         $"[{string.Join(", ", projectInfoBuilder.ProjectInfoEnrichers.Select(x => x.GetType().Name))}]");
    }


    private void DisplayParameters(TransmitterArguments args)
        {
            this.logger.Info($"Command line string: [{args.OriginalParameterInputString}]");
            this.logger.Info($"Resolved parameters:");

            foreach (KeyValuePair<string, string> parameter in args.GetParameterCollection())
            {
                this.logger.Info($"{parameter.Key}: [{parameter.Value}]");
            }
        }

        private IEnumerable<string> GetPaths(TransmitterArguments args, IInputUriProvider provider)
        {
            IEnumerable<string> uris;
            if (args.ProjectPaths == null || !args.ProjectPaths.Any())
            {
                Regex regex = null;
                if (!string.IsNullOrEmpty(args.IgnoredPathsRegex))
                {
                    regex = new Regex(args.IgnoredPathsRegex);
                }
                uris = provider.GetUris(args.CodeRootFolder,regex);
            }
            else
            {
                uris = args.ProjectPaths;
            }

            return uris;
        }
    }
}