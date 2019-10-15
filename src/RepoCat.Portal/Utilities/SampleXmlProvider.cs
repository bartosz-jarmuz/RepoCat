using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore.Internal;
using RepoCat.Transmission.Core;
using RepoCat.Transmission.Models;

namespace RepoCat.Portal.Utilities
{
    /// <summary>
    /// Generates a sample XML
    /// </summary>
    public static class SampleManifestXmlProvider
    {
        /// <summary>
        ///  Gets a sample component manifest XML
        /// </summary>
        /// <returns>List&lt;System.String&gt;.</returns>
        public static string GetProjectInfoSerialized()
        {
            var info = GetProjectInfo();
            return ManifestSerializer.SerializeProjectInfo(info).ToString();
        }

        /// <summary>
        /// Gets a sample pre-filled project info
        /// </summary>
        /// <returns></returns>
        public static ProjectInfo GetProjectInfo()
        {
            ProjectInfo info = new ProjectInfo()
            {
                ProjectName = "A name of the project",
                AssemblyName = "MyAssemblyName",
                TargetExtension = ".dll",
                OutputType = "Library",
                ProjectUri = "//local/or/remote/path/to/project.csproj",
                RepositoryName = "My Cool Projects",
                RepositoryStamp = "1.0.2929"
            };
            info.Components = new List<ComponentManifest>()
            {
                new ComponentManifest()
                {
                    Name = "Name of the component",
                    Description = "Short description of what this component does (e.g. validates XML)",
                    DocumentationUri = "//local/or/remote/path/to/documentation",
                    Tags = new List<string>()
                    {
                        "One",
                        "Or",
                        "More",
                        "Keywords"
                    },
                    Properties = new Dictionary<string, string>()
                    {
                        {"OtherProjectMetadata", "As key and value pairs" }
                    }
                },
                new ComponentManifest()
                {
                    Name = "XmlValidator",
                    Description = "Validates XML files against a custom XSD",
                    DocumentationUri = "http://mycoresoft.com/XmlValidator",
                    Tags = new List<string>()
                    {
                        "Xml",
                        "Validation",
                        "Schema"
                    },
                    Properties = new Dictionary<string, string>()
                    {
                        {"ComponentType", "Plugin" },
                        {"HostApplication", "Notepad++" },
                        {"AgeRestriction", "18+" },
                    }
                }
            };
            return info;
        }




    }
}
