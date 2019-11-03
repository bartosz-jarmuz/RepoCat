using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using log4net;
using RepoCat.ProjectFileReaders;
using RepoCat.ProjectFileReaders.ProjectModel;
using RepoCat.Transmission.Client.Interfaces;
using RepoCat.Transmission.Models;

namespace RepoCat.Transmission.Client.Implementation
{
    public class ProjectInfoProvider : IProjectInfoProvider
    {
        private readonly ILog log;

        private readonly string ManifestSuffix = "RepoCat.xml";

        public ProjectInfoProvider(ILog log)
        {
            this.log = log;
        }

        public IEnumerable<ProjectInfo> GetInfos(IEnumerable<string> uris, string repo, string repoStamp)
        {
            if (uris == null) throw new ArgumentNullException(nameof(uris));

            int counter = 0;
            foreach (string uri in uris)
            {
                this.log.Debug($"Checking project #{counter} for manifest file. {uri}");

                counter++;
                ProjectInfo info = this.GetInfo(uri, repo, repoStamp);
                if (info != null)
                {
                    yield return info;
                }
                this.log.Debug($"Project #{counter} does not contain a manifest file. {uri}");

            }
            this.log.Info($"Loaded project infos for {counter} projects.");

        }

        public ProjectInfo GetInfo(string uri, string repo, string repoStamp)
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
                    this.log.Debug($"Reading Project Info - {uri}");
                    ProjectInfo info = ConstructInfo(uri, repo, repoStamp, project);
                    if (info != null)
                    {
                        this.log.Debug($"Loaded project info. Reading manifest info from {uri}.");
                        this.LoadComponentManifest(uri, manifestInclude, info);
                        return info;
                    }

                }
                else
                {
                    this.log.Debug($"Project does not include manifest file (expected file name ending with [{this.ManifestSuffix}]). Will be ignored by RepoCat. {uri}");
                }
            }
            catch (Exception ex)
            {
                this.log.Error($"Unexpected error while loading project info for [{uri}] {ex.Message}. Details in DEBUG mode.");
                this.log.Debug($"Error details: {ex}.");
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
                    this.log.Error($"Manifest not found at [{manifestInclude.ResolvedIncludePath}] for project {uri}!");
                }
                else
                {
                    string manifestContent = File.ReadAllText(file.FullName);
                    info.Components.AddRange( ManifestDeserializer.DeserializeComponents(manifestContent));
                    this.log.Info($"Manifest Read OK from {uri}");
                }
            }
            catch (Exception ex)
            {
                this.log.Warn($"Error while loading ComponentManifest [{uri}] {ex.Message}. Details in DEBUG mode.");
                this.log.Debug($"Error details:" + ex);
            }
        }

        private ProjectInfo ConstructInfo(string uri, string repo, string repoStamp, Project prj)
        {
            try
            {
                ProjectInfo info = new ProjectInfo()
                {
                    AssemblyName = prj.AssemblyName,
                    ProjectUri = prj.FullPath,
                    ProjectName = Path.GetFileNameWithoutExtension(prj.Name),
                    RepositoryName = repo,
                    RepositoryStamp = repoStamp,
                    OutputType = prj.OutputType,
                    TargetExtension = prj.OutputType
                };
                return info;

            }
            catch (Exception ex)
            {
                this.log.Warn($"Error while constructing project info based on project object [{uri}] {ex.Message}. Details in DEBUG mode.");
                this.log.Debug($"Error details:" + ex);
                return null;
            }

        }

        private Project LoadProject(string uri)
        {
            Project prj;
            try
            {
                ProjectFileFactory factory = new ProjectFileFactory();
                prj = factory.GetProject(new FileInfo(uri));
                this.log.Debug($"Project loaded from [{uri}]");
            }

            catch (Exception ex)
            {
                this.log.Warn($"Error while loading project [{uri}] {ex.Message}. Details in DEBUG mode.");
                this.log.Debug($"Error details:" + ex);
                return null;
            }

            return prj;
        }
    }

    

}