using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using DotNetProjectParser;
using RepoCat.Transmission.Models;

namespace RepoCat.Transmission.Client
{
    public class DotNetProjectInfoProvider : ProjectInfoProviderBase
    {
        private readonly ILogger logger;

        public DotNetProjectInfoProvider(ILogger logger) : base(logger)
        {
            this.logger = logger;
        }


        public override ProjectInfo GetInfo(string projectUri)
        {
            Project project = this.LoadProject(projectUri);
            if (project == null)
            {
                return null;
            }

            try
            {
                ProjectItem manifestInclude = project.Items.FirstOrDefault(x => x.ResolvedIncludePath.EndsWith(Strings.ManifestSuffix, StringComparison.CurrentCultureIgnoreCase));
                if (manifestInclude?.ResolvedIncludePath != null)
                {
                    this.logger.Debug($"Reading Project Info - {projectUri}");
                    ProjectInfo info = this.ConstructInfo(projectUri, project);
                    if (info != null)
                    {
                        this.logger.Debug($"Loaded project info. Reading manifest info from {projectUri}.");
                        this.LoadComponentManifest(projectUri, manifestInclude, info);
                        foreach (IProjectInfoEnricher projectInfoEnricher in this.ProjectInfoEnrichers)
                        {
                            projectInfoEnricher.Enrich(info, manifestInclude.ResolvedIncludePath, projectUri);
                        }
                        return info;
                    }

                }
                else
                {
                    this.logger.Debug($"Project does not include manifest file (expected file name ending with [{Strings.ManifestSuffix}]). Will be ignored by RepoCat. {projectUri}");
                }
            }
            catch (Exception ex)
            {
                this.logger.Error($"Unexpected error while loading project info for [{projectUri}] {ex.Message}. Details in DEBUG mode.");
                this.logger.Debug($"Error details: {ex}.");
            }
            return null;
        }

        private void LoadComponentManifest(string projectUri, ProjectItem manifestInclude, ProjectInfo info)
        {
            try
            {
                FileInfo file = new FileInfo(manifestInclude.ResolvedIncludePath);
                if (!file.Exists)
                {
                    this.logger.Error($"Manifest not found at [{manifestInclude.ResolvedIncludePath}] for project {projectUri}!");
                }
                else
                {
                    XDocument manifest = XDocument.Load(file.FullName);
                    foreach (IProjectInfoEnricher projectInfoEnricher in this.ProjectInfoEnrichers)
                    {
                        projectInfoEnricher.Enrich(manifest, file.FullName, projectUri);
                    }
                    info.Components.AddRange( ManifestDeserializer.DeserializeComponents(manifest.Root));
                    this.logger.Info($"Manifest Read OK from {projectUri}");
                }
            }
            catch (Exception ex)
            {
                this.logger.Warn($"Error while loading ComponentManifest [{projectUri}] {ex.Message}. Details in DEBUG mode.");
                this.logger.Debug($"Error details:" + ex);
            }
        }

        private ProjectInfo ConstructInfo(string uri, Project prj)
        {
            try
            {
                ProjectInfo info = new ProjectInfo()
                {
                    AssemblyName = prj.AssemblyName,
                    ProjectUri = prj.FullPath,
                    ProjectName = Path.GetFileNameWithoutExtension(prj.Name),
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