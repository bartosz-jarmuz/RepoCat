using System.IO;
using System.Linq;
using FluentAssertions;
using log4net;
using Moq;
using RepoCat.Transmission.Client.Implementation;
using System.Xml.Linq;
using NUnit.Framework;
using RepoCat.ProjectFileReaders;

namespace RepoCat.ProjectParsers.Tests
{
    [TestFixture]
    public class ProjectParserTests
    {
        [Test]
        public void NetFrameworkProject_Load()
        {
            var fileInfo = TestUtils.GetSampleProject(@"RepoCat.TestApps.NetFramework.csproj");

            var fileFactory = new ProjectFileFactory();
            var project = fileFactory.GetProject(fileInfo);

            project.Should().NotBeNull();
            project.Name.Should().Be("RepoCat.TestApps.NetFramework.csproj");
            project.FullPath.Should().Be(fileInfo.FullName);
            project.DirectoryPath.Should().Be(fileInfo.Directory.FullName);
            project.AssemblyName.Should().Be("RepoCat.TestApps.NetFramework");
            project.OutputType.Should().Be("Exe");
            project.TargetExtension.Should().Be(".exe");
            project.TargetFramework.Should().Be("v4.7");

            project.Items.Should().ContainEquivalentOf(new ProjectItem()
            {
                Include = "Program.cs",
                EvaluatedInclude = Path.Combine(fileInfo.Directory.FullName, "Program.cs"),
                ItemType = "Compile",
                CopyToOutputDirectory = null
            });
            project.Items.Should().ContainEquivalentOf(new ProjectItem()
            {
                Include = @"Properties\Manifest.RepoCat.xml",
                EvaluatedInclude = Path.Combine(fileInfo.Directory.FullName, "Properties", "Manifest.RepoCat.xml"),
                ItemType = "None",
                CopyToOutputDirectory = "Always"

            });
        }

        [Test]
        public void NetCoreProject_Load()
        {
            var path = TestUtils.GetSampleProject(@"RepoCat.TestApps.NetCore.csproj");

            var provider = new ProjectInfoProvider(new Mock<ILog>().Object);

            var info = provider.GetInfo(path.FullName, "Test", "");
            info.Should().NotBeNull();
            info.AssemblyName.Should().Be("RepoCat.TestApps.NetCore");
            info.Components.Single().Name.Should().Be("SampleNetCoreConsoleApp");
            info.Components.Single().Tags.Count.Should().Be(3);
        }
    }
}
