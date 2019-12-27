using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DotNetProjectParser;
using RepoCat.Transmission.Models;

namespace RepoCat.Transmission.Client
{
    public class ProjectInfoProvider : IProjectInfoProvider
    {
        private readonly ILogger logger;

        private readonly string ManifestSuffix = "RepoCat.xml";

        public ProjectInfoProvider(ILogger logger)
        {
            this.logger = logger;
        }

        public IEnumerable<ProjectInfo> GetInfos(IEnumerable<string> uris, RepositoryInfo repositoryInfo, string repoStamp)
        {
            if (uris == null) throw new ArgumentNullException(nameof(uris));

            int counter = 0;
            foreach (string uri in uris)
            {
                this.logger.Debug($"Checking project #{counter} for manifest file. {uri}");

                counter++;
                ProjectInfo info = this.GetInfo(uri, repositoryInfo, repoStamp);
                if (info != null)
                {
                    yield return info;
                }
                this.logger.Debug($"Project #{counter} does not contain a manifest file. {uri}");

            }
            this.logger.Info($"Loaded project infos for {counter} projects.");

        }

        public ProjectInfo GetInfo(string uri, RepositoryInfo repositoryInfo, string repoStamp)
        {
            Project project = this.LoadProject(uri);
            if (project == null)
            {
                return null;
            }

            try
            {
                ProjectItem manifestInclude = project.Items.FirstOrDefault(x => x.ResolvedIncludePath.EndsWith(this.ManifestSuffix, StringComparison.CurrentCultureIgnoreCase));
                if (manifestInclude?.ResolvedIncludePath != null)
                {
                    this.logger.Debug($"Reading Project Info - {uri}");
                    ProjectInfo info = this.ConstructInfo(uri, repositoryInfo, repoStamp, project);
                    if (info != null)
                    {
                        this.logger.Debug($"Loaded project info. Reading manifest info from {uri}.");
                        this.LoadComponentManifest(uri, manifestInclude, info);
                        return info;
                    }

                }
                else
                {
                    this.logger.Debug($"Project does not include manifest file (expected file name ending with [{this.ManifestSuffix}]). Will be ignored by RepoCat. {uri}");
                }
            }
            catch (Exception ex)
            {
                this.logger.Error($"Unexpected error while loading project info for [{uri}] {ex.Message}. Details in DEBUG mode.");
                this.logger.Debug($"Error details: {ex}.");
            }
            return null;
        }

        private void LoadComponentManifest(string uri, ProjectItem manifestInclude, ProjectInfo info)
        {
            try
            {
                FileInfo file = new FileInfo(manifestInclude.ResolvedIncludePath);
                if (!file.Exists)
                {
                    this.logger.Error($"Manifest not found at [{manifestInclude.ResolvedIncludePath}] for project {uri}!");
                }
                else
                {
                    string manifestContent = File.ReadAllText(file.FullName);
                    info.Components.AddRange( ManifestDeserializer.DeserializeComponents(manifestContent));
                    this.logger.Info($"Manifest Read OK from {uri}");
                }
            }
            catch (Exception ex)
            {
                this.logger.Warn($"Error while loading ComponentManifest [{uri}] {ex.Message}. Details in DEBUG mode.");
                this.logger.Debug($"Error details:" + ex);
            }
        }

        private ProjectInfo ConstructInfo(string uri, RepositoryInfo repositoryInfo, string repoStamp, Project prj)
        {
            try
            {
                ProjectInfo info = new ProjectInfo()
                {
                    AssemblyName = prj.AssemblyName,
                    ProjectUri = prj.FullPath,
                    ProjectName = Path.GetFileNameWithoutExtension(prj.Name),
                    RepositoryInfo = repositoryInfo,
                    RepositoryStamp = repoStamp,
                    OutputType = prj.OutputType,
                    TargetExtension = prj.TargetExtension
                };
                return info;

            }
            catch (Exception ex)
            {
                this.logger.Warn($"Error while constructing project info based on project object [{uri}] {ex.Message}. Details in DEBUG mode.");
                this.logger.Debug($"Error details:" + ex);
                return null;
            }

        }

        private Project LoadProject(string uri)
        {
            Project prj;
            try
            {
                
                prj = ProjectFactory.GetProject(new FileInfo(uri));
                this.logger.Debug($"Project loaded from [{uri}]");
            }

            catch (Exception ex)
            {
                this.logger.Warn($"Error while loading project [{uri}] {ex.Message}. Details in DEBUG mode.");
                this.logger.Debug($"Error details:" + ex);
                return null;
            }

            return prj;
        }
    }

    

}