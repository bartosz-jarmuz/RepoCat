using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;
using Moq;
using System.Xml.Linq;
using NUnit.Framework;
using RepoCat.Transmission;
using RepoCat.Transmission.Builders.Excel;
using RepoCat.Transmission.Contracts;
using RepoCat.Transmission.Models;

namespace RepoCat.ProjectParsers.Tests
{
    [TestFixture]
    public class ExcelProjectInfoBuilderTests
    {
        private static DirectoryInfo FilesDirectory => new DirectoryInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "Files"));

        [Test]
        public void ExcelFile_Load()
        {
            //arrange
            var file = FilesDirectory.GetFiles("Apps.xlsm").First();
            var mapping = new Dictionary<string, string>()
            {
                { "Tool Name", nameof(ProjectInfo.ProjectName) },
                { "Purpose of Tool", nameof(ProjectInfo.ProjectDescription) },
                { "MyCompany Developed or External", nameof(ProjectInfo.Owner) },
            };
            var builder = new ExcelBasedProjectInfoBuilder(new TraceLogger(LogLevel.Debug), mapping);
            
            //act
            var infos = builder.GetInfos(new[] {file.FullName}).ToList();


            //assert
            Assert.AreEqual(2, infos.Count);
            var projectInfo = infos[0];
            Assert.AreEqual("ADD_AND_REMOVE_COMMENT", projectInfo.ProjectName);
            Assert.AreEqual("Add MyCompany comment and changes the encoding of the files.\n" +
                            "Removes MyCompany comment and changes the encoding of the files.", projectInfo.ProjectDescription);
            Assert.AreEqual("MyCompany Developed",projectInfo.Owner);
            CollectionAssert.AreEquivalent(new []{ "emediot", "add", "remove", "add_and_remove" },projectInfo.Tags);
            Assert.AreEqual(7,projectInfo.Properties.Count);

            Assert.AreEqual("XML",projectInfo.Properties["Application"]);
            Assert.AreEqual("John Doe",projectInfo.Properties["Developer"]);
            Assert.AreEqual("Not necessarily", projectInfo.Properties["Client specific?"]);
            Assert.AreEqual("ACME",projectInfo.Properties["Name of client?"]);
            Assert.AreEqual("ENG",projectInfo.Properties["Which department it supports?"]);
            Assert.AreEqual(@"\\mhdfiler03\fs_mhd\scripts\TD\APPS\XML\Add_and_Remove_MyCompanyComment",projectInfo.Properties["Path to App"]);
            Assert.IsNotNull( projectInfo.Properties["Add date"]);

            projectInfo = infos[1];
            Assert.AreEqual("ADJUST_FONT_SIZES", projectInfo.ProjectName);
            Assert.AreEqual("The macro will help fit the text in multiple word files at once. Recommended if page to page is required.", projectInfo.ProjectDescription);
            Assert.AreEqual("External", projectInfo.Owner);
            CollectionAssert.AreEquivalent(new[] { "word", "doc", "docx", "adjust font size"}, projectInfo.Tags);
            Assert.AreEqual(7, projectInfo.Properties.Count);

            Assert.AreEqual("MICROSOFT WORD", projectInfo.Properties["Application"]);
            Assert.AreEqual("Jim Beam", projectInfo.Properties["Developer"]);
            Assert.AreEqual("No", projectInfo.Properties["Client specific?"]);
            Assert.AreEqual("", projectInfo.Properties["Name of client?"]);
            Assert.AreEqual("DTP", projectInfo.Properties["Which department it supports?"]);
            Assert.AreEqual(@"\\mhdfiler03\fs_mhd\scripts\TD\APPS\MICROSOFT_WORD\Adjust_Font_Sizes", projectInfo.Properties["Path to App"]);
            Assert.IsNotNull(projectInfo.Properties["Add date"]);

        }
    }
}
