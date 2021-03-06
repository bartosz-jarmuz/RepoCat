﻿// -----------------------------------------------------------------------
//  <copyright file="SampleXmlProvider.cs" company="bartosz.jarmuz@gmail.com">
//   Copyright (c) Bartosz Jarmuż. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Xml.Linq;
using RepoCat.Serialization;
using RepoCat.Transmission.Contracts;
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
            info.RepositoryInfo.RepositoryName = "MISC";
            info.RepositoryInfo.OrganizationName = "MISC";
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
                    new PropertiesCollection()
                    {
                        new Property("","")
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
                Owner = "DreamTeam Developers",
                DocumentationUri = "http://maindocumentation.url/docs",
                ProjectDescription = "Optional Description of a project",
                Tags = { "Some", "Optional", "Project", "Tags"},
                Properties = { {"ProjectProperty", "ValueWhichCouldAlsoBeAlist"}},
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
                    }, new PropertiesCollection()
                    {
                        ("OtherProjectMetadata", "As key and value pairs"),
                        ("FewNumbers", new[]{"One", "Two", "Three"}),
                        ("FewWords", new[]{"Car", "Banana", "Hamack"}),
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
                    new PropertiesCollection()
                    {
                        ("ComponentType", "Plugin"),
                        ("HostApplication", "Notepad++"),
                        ("AgeRestriction", "18+"),
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