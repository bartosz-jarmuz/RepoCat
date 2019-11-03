﻿using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using RepoCat.ProjectFileReaders.ProjectModel;
using RepoCat.ProjectFileReaders.Readers;

namespace RepoCat.ProjectFileReaders
{
    /// <summary>
    /// Creates an instance of project object regardless of the framework
    /// </summary>
    public class ProjectFileFactory : IProjectFileFactory
    {
        /// <summary>
        /// Gets an instance 
        /// </summary>
        /// <param name="projectFile"></param>
        /// <returns></returns>
        public Project GetProject(FileInfo projectFile)
        {
            if (projectFile == null) throw new ArgumentNullException(nameof(projectFile));

            var projectDocument = LoadDocument(projectFile);
            
            IXmlProjectFileReader reader =  this.GetXmlReader(projectDocument);

            return reader.ReadFile(projectFile, projectDocument);
        }

        private IXmlProjectFileReader GetXmlReader(XDocument projectDocument)
        {
            var toolsVersionAttribute = projectDocument.Root?.Attributes().FirstOrDefault(x => x.Name.LocalName == "ToolsVersion");
            var namespaceAttribute = projectDocument.Root?.Attributes().FirstOrDefault(x => x.Name.LocalName == "xmlns");
            
            bool probablyFramework = (toolsVersionAttribute != null || 
                                      (namespaceAttribute != null && namespaceAttribute.Value.Equals("http://schemas.microsoft.com/developer/msbuild/2003", StringComparison.OrdinalIgnoreCase)));


            var skdAttribute = projectDocument.Root?.Attributes().FirstOrDefault(x => x.Name.LocalName == "Sdk");
            bool probablyCore = skdAttribute != null;


            if (probablyFramework  && !probablyCore)
            {
                return new NetFrameworkProjectReader();
            }
            else if (!probablyFramework && probablyCore)
            {
                return new NetCoreProjectReader();
            }
            else
            {
                throw new ProjectParserException("Cannot determine the project framework type");
            }
        }


        internal static XDocument LoadDocument(FileInfo projectFile)
        {
            XDocument xml;

            try
            {
                xml = XDocument.Load(projectFile.FullName);
            }
            catch (Exception ex)
            {
                throw new ProjectParserException("Failed to load project file as XML", projectFile.FullName, typeof(ProjectFileFactory), ex);
            }

            if (xml.Root == null)
            {
                throw new ProjectParserException("Empty XML Document!", projectFile.FullName, typeof(ProjectFileFactory));
            }

            return xml;
        }
    }
}