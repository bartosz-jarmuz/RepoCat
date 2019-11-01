using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using log4net;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Locator;
using RepoCat.Transmission.Client.Interface;
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
            if (MSBuildLocator.CanRegister)
            {
                MSBuildLocator.RegisterDefaults();
            }
        }

        public IEnumerable<ProjectInfo> GetInfos(IEnumerable<string> uris, string repo, string repoStamp)
        {
            var counter = 0;
            foreach (string uri in uris)
            {
                this.log.Debug($"Checking project #{counter} for manifest file. {uri}");

                counter++;
                var info = this.GetInfo(uri, repo, repoStamp);
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
                var manifestInclude = project.Items.FirstOrDefault(x => x.EvaluatedInclude.EndsWith(this.ManifestSuffix, StringComparison.CurrentCultureIgnoreCase));
                if (manifestInclude != null)
                {
                    this.log.Debug($"Reading Project Info - {uri}");
                    ProjectInfo info = ConstructInfo(uri,repo, repoStamp, project);
                    if (info != null)
                    {
                        this.log.Debug($"Loaded project info. Reading manifest info from {uri}.");
                        this.LoadComponentManifest(uri, project, manifestInclude, info);
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

        private void LoadComponentManifest(string uri, Project project, ProjectItem manifestInclude, ProjectInfo info)
        {
            try
            {

                string manifestPath = Directory
                    .GetFiles(project.DirectoryPath, manifestInclude.EvaluatedInclude, SearchOption.AllDirectories)
                    .FirstOrDefault();
                if (string.IsNullOrEmpty(manifestPath))
                {
                    this.log.Error($"Manifest not found at [{manifestInclude.EvaluatedInclude}] for project {uri}!");
                }
                else
                {
                    string manifestContent = File.ReadAllText(manifestPath);
                    info.Components = ManifestDeserializer.DeserializeComponents(manifestContent);
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
                var info = new ProjectInfo()
                {
                    AssemblyName = prj.Properties
                        .FirstOrDefault(x => x.Name.Equals("AssemblyName", StringComparison.CurrentCultureIgnoreCase))
                        ?.EvaluatedValue,
                    ProjectUri = prj.FullPath,
                    ProjectName = Path.GetFileNameWithoutExtension(prj.FullPath),
                    RepositoryName = repo,
                    RepositoryStamp = repoStamp,
                    OutputType = prj.Properties
                        .FirstOrDefault(x => x.Name.Equals("OutputType", StringComparison.CurrentCultureIgnoreCase))
                        ?.EvaluatedValue,
                    TargetExtension = prj.Properties
                        .FirstOrDefault(x => x.Name.Equals("TargetExt", StringComparison.CurrentCultureIgnoreCase))
                        ?.EvaluatedValue,
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
                prj = new Project(uri);
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