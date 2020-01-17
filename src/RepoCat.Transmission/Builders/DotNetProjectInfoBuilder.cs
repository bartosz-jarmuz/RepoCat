using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml.Linq;
using DotNetProjectParser;
using RepoCat.Serialization;
using RepoCat.Transmission.Contracts;
using RepoCat.Transmission.Models;

namespace RepoCat.Transmission
{
    public class DotNetProjectInfoBuilder : ProjectInfoBuilderBase
    {
        private readonly ILogger logger;
        private readonly TransmitterArguments args;

        public DotNetProjectInfoBuilder(ILogger logger, TransmitterArguments args) : base(logger)
        {
            this.logger = logger;
            this.args = args;
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
                    return this.GetInfoWithManifest(projectUri, manifestInclude, project);
                }
                else
                {
                    return this.GetInfoWithoutManifest(projectUri, project);
                }
            }
            catch (Exception ex)
            {
                this.logger.Error($"Unexpected error while loading project info for [{projectUri}] {ex.Message}. Details in DEBUG mode.");
                this.logger.Debug($"Error details: {ex}.");
            }
            return null;
        }

        private ProjectInfo GetInfoWithoutManifest(string projectUri, Project project)
        {
            if (this.args.SkipProjectsWithoutManifest)
            {
                this.logger.Debug(
                    $"Project does not include manifest file (expected file name ending with [{Strings.ManifestSuffix}]) " +
                    $"and [{nameof(this.args.SkipProjectsWithoutManifest)}] setting is [{this.args.SkipProjectsWithoutManifest}]. Will be ignored by RepoCat. {projectUri}");
                return null;
            }
            else
            {
                this.logger.Debug(
                    $"Preparing project without manifest file as per [{nameof(this.args.SkipProjectsWithoutManifest)}] setting value: [{this.args.SkipProjectsWithoutManifest}]");
                ProjectInfo projectInfoReadFromCsproj = this.ConstructInfo(projectUri, project);

                this.ProcessProjectEnriching(projectUri, projectInfoReadFromCsproj, null);
                return projectInfoReadFromCsproj;
            }
        }

        private ProjectInfo GetInfoWithManifest(string projectUri, ProjectItem manifestInclude, Project project)
        {
            this.logger.Debug($"Reading manifest - {manifestInclude.ResolvedIncludePath}");
            XDocument manifest = XDocument.Load(manifestInclude.ResolvedIncludePath);

            this.ProcessManifestEnriching(projectUri, manifest, manifestInclude);

            ProjectInfo projectInfoFromManifest = ManifestDeserializer.DeserializeProjectInfo(manifest.Root);

            this.SynchronizeProjectInfos(projectInfoFromManifest, projectUri, project);

            this.ProcessProjectEnriching(projectUri, projectInfoFromManifest, manifestInclude.ResolvedIncludePath);

            return projectInfoFromManifest;
        }

        private void ProcessProjectEnriching(string projectUri, ProjectInfo projectInfoFromManifest, string manifestPath)
        {
            foreach (IProjectInfoEnricher projectInfoEnricher in this.ProjectInfoEnrichers)
            {
                projectInfoEnricher.Enrich(projectInfoFromManifest, manifestPath, projectUri);
            }
        }

        private void ProcessManifestEnriching(string projectUri, XDocument manifest, ProjectItem manifestInclude)
        {
            foreach (IProjectInfoEnricher projectInfoEnricher in this.ProjectInfoEnrichers)
            {
                projectInfoEnricher.Enrich(manifest, manifestInclude?.ResolvedIncludePath, projectUri);
            }
        }

        /// <summary>
        /// Ensure that whatever property was set in the manifest (which is done explicitly and manually) prevails over the automatically gathered info.
        /// </summary>
        /// <param name="projectInfoFromManifest"></param>
        /// <param name="projectUri"></param>
        /// <param name="project"></param>
        private void SynchronizeProjectInfos(ProjectInfo projectInfoFromManifest, string projectUri, Project project)
        {
            ProjectInfo projectInfoReadFromCsproj = this.ConstructInfo(projectUri, project);

            var properties = typeof(ProjectInfo).GetProperties().Where(x => x.Name != nameof(ProjectInfo.Components));

            foreach (var propertyInfo in properties)
            {
                object manifestValue = propertyInfo.GetValue(projectInfoFromManifest);
                object defaultValue = propertyInfo.PropertyType.IsValueType ? Activator.CreateInstance(propertyInfo.PropertyType) : null;

                if (manifestValue == defaultValue)
                {
                    propertyInfo.SetValue(projectInfoFromManifest, propertyInfo.GetValue(projectInfoReadFromCsproj));
                }
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
                this.logger.Error($"Error while constructing project info based on project object [{uri}] {ex.Message}. Details in DEBUG mode.", ex);
                throw;
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