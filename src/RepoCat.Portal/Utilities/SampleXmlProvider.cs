using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using RepoCat.Serialization;
using RepoCat.Transmission;
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
        public static string GetSampleProjectInfoSerialized()
        {
            ProjectInfo info = GetSampleProjectInfo();
            XElement xElement = ManifestSerializer.SerializeProjectInfo(info);
            return xElement.ToString();
        }

        /// <summary>
        ///  Gets an empty component manifest XML
        /// </summary>
        /// <returns>List&lt;System.String&gt;.</returns>
        public static string GetEmptyProjectInfoSerialized()
        {
            ProjectInfo info = GetEmptyProjectInfo();
            XElement xElement = ManifestSerializer.SerializeProjectInfo(info);
            return xElement.ToString();
        }

        /// <summary>
        /// Gets a sample pre-filled project info
        /// </summary>
        /// <returns></returns>
        public static ProjectInfo GetEmptyProjectInfo()
        {
            ProjectInfo info = new ProjectInfo()
            {
                ProjectName = " ",
                AssemblyName = " ",
                TargetExtension = " ",
                OutputType = " ",
                ProjectUri = " ",
                DownloadLocation = " ",
                RepositoryStamp = " ",
                RepositoryInfo = new RepositoryInfo()
                {
                    RepositoryName = " ",
                    OrganizationName = " "
                }
            };


            info.Components.AddRange(new List<ComponentManifest>()
            {
                new ComponentManifest(new List<string>()
                    {
                        "",
                    },
                    new Dictionary<string, string>()
                    {
                        {"", ""}
                    }
                )
                {
                    Name = " ",
                    Description = " ",
                    DocumentationUri = " ",
                },
            });
            return info;
        }

        /// <summary>
        /// Gets a sample pre-filled project info
        /// </summary>
        /// <returns></returns>
        public static ProjectInfo GetSampleProjectInfo()
        {
            ProjectInfo info = new ProjectInfo()
            {
                ProjectName = "A name of the project",
                AssemblyName = "MyAssemblyName",
                TargetExtension = ".dll",
                OutputType = "Library",
                ProjectUri = "//local/or/remote/path/to/project.csproj",
                DownloadLocation = "//networkshare/or/url/path/to/program",
                RepositoryInfo = new RepositoryInfo()
                {
                    RepositoryName = "CoolProjects",
                    OrganizationName = "DreamTeam",
                },
                RepositoryStamp = "1.0.2929"
            };
            info.Components.AddRange(new List<ComponentManifest>()
            {
                new ComponentManifest(new List<string>()
                    {
                        "One",
                        "Or",
                        "More",
                        "Keywords"
                    }, new Dictionary<string, string>()
                    {
                        {"OtherProjectMetadata", "As key and value pairs"}
                    }
                )
                {
                    Name = "Name of the component",
                    Description = "Short description of what this component does (e.g. validates XML)",
                    DocumentationUri = "//local/or/remote/path/to/documentation",
                },
                new ComponentManifest(new List<string>()
                {
                    "Xml",
                    "Validation",
                    "Schema"
                },
                    new Dictionary<string, string>()
                    {
                        {"ComponentType", "Plugin"},
                        {"HostApplication", "Notepad++"},
                        {"AgeRestriction", "18+"},
                    }
                    )
                {
                    Name = "XmlValidator",
                    Description = "Validates XML files against a custom XSD",
                    DocumentationUri = "http://mycoresoft.com/XmlValidator",
                }
            });
            return info;
        }
    }
}