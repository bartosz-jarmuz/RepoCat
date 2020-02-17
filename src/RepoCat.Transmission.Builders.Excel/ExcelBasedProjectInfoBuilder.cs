// -----------------------------------------------------------------------
//  <copyright file="ExcelBasedProjectInfoBuilder.cs" company="bartosz.jarmuz@gmail.com">
//   Copyright (c) Bartosz Jarmuż. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using ExcelDataReader;
using RepoCat.Transmission.Contracts;
using RepoCat.Transmission.Models;

namespace RepoCat.Transmission.Builders.Excel
{
    /// <summary>
    /// Provides project info based on rows in excel file, without parsing a project file
    /// </summary>
    public class ExcelBasedProjectInfoBuilder : ProjectInfoBuilderBase
    {
        private readonly ILogger logger;
        private readonly Dictionary<string, string> settings;
        private readonly List<PropertyInfo> projectInfoProperties = typeof(ProjectInfo).GetProperties().ToList();


        /// <summary>
        /// Creates new instance
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="projectEnrichers"></param>
        /// <param name="settings">Mapping of Excel column header to ProjectInfo class property name</param>
        public ExcelBasedProjectInfoBuilder(ILogger logger, IProjectEnrichersFunnel projectEnrichers, Dictionary<string, string> settings) : base(logger, projectEnrichers)
        {
            this.logger = logger;
            this.settings = settings;
        }

        public override IEnumerable<ProjectInfo> GetInfos(IEnumerable<string> uris)
        {
            foreach (string uri in uris)
            {
                if (IsExcelChecker.IsExcelFile(uri))
                {
                    DataSet dataSet = this.GetExcelData(uri);
                    foreach (ProjectInfo projectInfo in this.GetInfoFromRows(dataSet))
                    {
                        this.ProjectInfoEnrichers.EnrichProject(uri, projectInfo, uri, dataSet);
                        yield return projectInfo;
                    }
                }
                else
                {
                    throw new ArgumentException($"The type of file is not supported. Expected Excel file. {uri}");
                }
            }
        }

        private DataSet GetExcelData(string path)
        {
            using (var reader = ExcelReaderFactory.CreateReader(File.OpenRead(path)))
            {
                return reader.AsDataSet();
            }
        }

        private IEnumerable<ProjectInfo> GetInfoFromRows(DataSet dataSet)
        {
            DataRow headersRow = dataSet.Tables[0].Rows[0];

            for (int index = 1; index < dataSet.Tables[0].Rows.Count; index++)
            {
                DataRow dataRow = dataSet.Tables[0].Rows[index];
                yield return this.GetInfoFromRow(dataRow, headersRow);
            }
        }

        private PropertyInfo GetProjectPropertyInfoForCell(DataRow headersRow, int index, out string headerLabel)
        {
            //excel contains columns with various names - map the values specified in the settings to name of properties of the projectInfo class
            headerLabel = headersRow[index].ToString();
            if (this.settings.TryGetValue(headerLabel, out string value))
            {
                var propertyMappedInSettings = this.projectInfoProperties.FirstOrDefault(x => x.Name.Equals(value, StringComparison.OrdinalIgnoreCase));
                if (propertyMappedInSettings != null)
                {
                    return propertyMappedInSettings;
                }
            }
            else
            {
                //maybe a column header in excel is already identical as the ProjectInfo class property
                var implicitlyMappedProperty = this.projectInfoProperties.FirstOrDefault(x => x.Name.Equals(headersRow[index].ToString(), StringComparison.OrdinalIgnoreCase));
                if (implicitlyMappedProperty != null)
                {
                    return implicitlyMappedProperty;
                }
            }
            //any column that contains data which is not a property of project info class or a column that is not set to be mapped should go as property property
            return this.projectInfoProperties.Single(x => x.Name == nameof(ProjectInfo.Properties));
        }
        

        private ProjectInfo GetInfoFromRow(DataRow row, DataRow headersRow)
        {
            var project = new ProjectInfo();

            for (int index = 0; index < row.ItemArray.Length; index++)
            {
                PropertyInfo propertyInfo = this.GetProjectPropertyInfoForCell(headersRow, index, out string headerLabel);
                if (propertyInfo.Name != nameof(ProjectInfo.Properties) && propertyInfo.Name != nameof(ProjectInfo.Tags))
                {
                    propertyInfo.SetValue(project, row[index]);
                }
                else
                {
                    if (propertyInfo.Name == nameof(ProjectInfo.Tags))
                    {
                        string[] split = row[index].ToString()?.Split(new[] {',', ';'});
                        project.Tags.AddRange(split.Select(x=>x.Trim('\'', '"', ' ')));
                    }
                    else
                    {
                        project.Properties.Add(headerLabel, row[index].ToString().Trim());
                    }
                }
            }

            return project;
        }

        
        protected override ProjectInfo GetInfo(string inputUri)
        {
            throw new NotImplementedException($"Excel builder expects the data to be accessed with the {nameof(this.GetInfos)} method");
        }
    }
}
