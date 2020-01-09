using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using RepoCat.Transmission.Models;

namespace RepoCat.Transmission
{
    /// <summary>
    /// Default implementation of transmitter. Contains extension points to make in customizable
    /// </summary>
    public class Transmitter : IProjectInfoTransmitter
    {
        private readonly ILogger logger;
        private readonly IProjectInfoSender projectInfoSender;

        /// <summary>
        /// Creates new instance
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="projectInfoSender">The component that performs actual delivery of generated project info to the API (via HTTP or directly to DB)</param>
        public Transmitter(ILogger logger, IProjectInfoSender projectInfoSender)
        {
            this.logger = logger;
            this.projectInfoSender = projectInfoSender;
        }

        ///<inheritdoc cref="IProjectInfoTransmitter"/>
        public Task Work(string[] args)
        {
            TransmitterArguments arguments= new TransmitterArguments(args);
            this.logger.Debug($"Arguments: {arguments.OriginalParameterInputString}");
            return this.Work(arguments);
        }

        ///<inheritdoc cref="IProjectInfoTransmitter"/>

        public IList<IProjectInfoEnricher> AdditionalProjectInfoEnrichers { get; } = new List<IProjectInfoEnricher>();

    ///<inheritdoc cref="IProjectInfoTransmitter"/>
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

                this.projectInfoSender.SetBaseAddress(args.ApiBaseUri);

                var result =  await this.projectInfoSender.Send(infos).ConfigureAwait(false);

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
                this.logger.Info($"Loading files from [{args.CodeRootFolder}], excluding those which match regex [{args.IgnoredPathsRegex}]");
                this.CheckIfCanAccesDirectory(args.CodeRootFolder, FileSystemRights.Read);
                uris = provider.GetUris(args.CodeRootFolder,regex);

            }
            else
            {
                this.logger.Info($"Files will be loaded from [{args.ProjectPaths.Count}] paths specified in the arguments.");
                uris = args.ProjectPaths;
            }

            return uris;
        }

        public bool CheckIfCanAccesDirectory(string path, FileSystemRights rights)
        {
            if (string.IsNullOrEmpty(path)) return false;

            try
            {
                AuthorizationRuleCollection rules = Directory.GetAccessControl(path)
                    .GetAccessRules(true, true, typeof(System.Security.Principal.SecurityIdentifier));
                WindowsIdentity identity = WindowsIdentity.GetCurrent();

                foreach (FileSystemAccessRule rule in rules)
                {
                    if (identity.Groups.Contains(rule.IdentityReference))
                    {
                        if ((rights & rule.FileSystemRights) == rights)
                        {
                            if (rule.AccessControlType == AccessControlType.Allow)
                            {
                                this.logger.Debug($"Confirmed access rights to [{path}]");
                                return true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.logger.Error($"Cannot access directory [{path}]", ex);
                throw;
            }
            this.logger.Error($"No permissions to directory [{path}]");
            throw new UnauthorizedAccessException($"No permissions to directory [{path}]");
        }
    }
}