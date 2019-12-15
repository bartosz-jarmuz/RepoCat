using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using MongoDB.Bson;
using MongoDB.Driver;
using NUnit.Framework;
using RepoCat.Persistence.Models;
using RepoCat.Persistence.Service;
using RepoCat.Portal.Mapping;
using RepoCat.RepositoryManagement.Service;

namespace Repocat.Persistence.Tests
{
    [TestFixture]
    public class GetProjectTests
    {
        private static readonly IRepoCatDbSettings Settings = GetSettings();

        static IRepoCatDbSettings GetSettings()
        {
            return new RepoCatDbSettings()
            {
                ProjectsCollectionName = "Projects",
                RepositoriesCollectionName = "Repositories",
                ConnectionString = "mongodb://localhost:27017",
                DatabaseName = "RepoCatDbTESTS"
            };
        }

        private readonly RepositoryInfo testRepoOne = new RepositoryInfo()
        {
            Id = ObjectId.GenerateNewId(),
            RepositoryName = "TestRepoOne",
            OrganizationName = "TestOrg"

        };

        private readonly RepositoryInfo testRepoTwo = new RepositoryInfo()
        {
            Id = ObjectId.GenerateNewId(),
            RepositoryName = "TestRepoTwo",
            OrganizationName = "TestOrg"

        };


        [SetUp]
        public void SetUp()
        {
            MongoClient client = new MongoClient(Settings.ConnectionString);
            client.DropDatabase(Settings.DatabaseName);

            RepositoryDatabase service = new RepositoryDatabase(Settings);
            RepositoryInfo result = service.Create(this.testRepoOne);
            Assert.IsNotNull(result);
            RepositoryInfo result2 = service.Create(this.testRepoTwo);
            Assert.IsNotNull(result2);

        }

        [TearDown]
        public void TearDown()
        {

        }


        [Test]
        public async Task TestGetProjectByQuery_DefaultRepo_VariousRepos_DoNotFilterByRepo_ShouldReturnTwo()
        {
            RepositoryDatabase service = new RepositoryDatabase(Settings);
            
            ProjectInfo prj = new ProjectInfo()
            {
                ProjectName = "Project1",
                ProjectUri = "SomeLocation",
                RepositoryId = this.testRepoOne.Id,
                TargetExtension = "exe",
                Components = { new ComponentManifest()
                {
                    Tags = new List<string>(){"FindMe"}
                }}
            };
            await service.Upsert(prj).ConfigureAwait(false);
            ProjectInfo prj2 = new ProjectInfo()
            {
                ProjectName = "Project2",
                ProjectUri = "SomeLocation",
                RepositoryId = this.testRepoTwo.Id,
                TargetExtension = "exe",
                Components = { new ComponentManifest()
                {
                    Tags = new List<string>(){"FindMe"}
                }}
            };
            await service.Upsert(prj2).ConfigureAwait(false);

            ProjectInfo prj3 = new ProjectInfo()
            {
                ProjectName = "Project3",
                ProjectUri = "SomeLocation",
                RepositoryId = this.testRepoTwo.Id,
                TargetExtension = "dll",
                Components = { new ComponentManifest()
                {
                    Tags = new List<string>(){"ButNotMe"}
                }}
            };
            await service.Upsert(prj3).ConfigureAwait(false);

            List<Project> result = (await service.GetProjectsByQuery("findme", false).ConfigureAwait(false)).ToList();

            result.Count.Should().Be(2, "because we are not filtering by repository");
            var returnedPrj1 = result.Single(x => x.ProjectInfo.ProjectName == "Project1");
            returnedPrj1.RepositoryInfo.RepositoryName.Should().Be("TestRepoOne");
            var returnedPrj2 = result.Single(x => x.ProjectInfo.ProjectName == "Project2");
            returnedPrj2.RepositoryInfo.RepositoryName.Should().Be("TestRepoTwo");

        }

        [Test]
        public async Task TestGetProject_TextFilter()
        {
            void AssertProjectFoundBy(string @by, ManifestQueryResult result1, ProjectInfo projectInfo)
            {
                result1.Projects.Count.Should().Be(1, $"searching by {by} part should work and return a single project");
                result1.Projects.Single().ProjectInfo.Id.Should().Be(projectInfo.Id, $"searching by {by} part should return the right project");
            }

            async Task<ManifestQueryResult> GetResult(RepositoryManagementService repositoryManagementService, string query)
            {
                ManifestQueryResult manifestQueryResult = await repositoryManagementService.GetCurrentProjects(
                    this.testRepoOne.OrganizationName.ToUpperInvariant(),
                    this.testRepoOne.RepositoryName.ToUpperInvariant(), query, false).ConfigureAwait(false);
                return manifestQueryResult;
            }

            RepositoryDatabase database = new RepositoryDatabase(Settings);
            var service = new RepositoryManagementService(database, new Mapper(MappingConfigurationFactory.Create()));

            var prj = new RepoCat.Transmission.Models.ProjectInfo()
            {
                ProjectName = "AcmeCorp.ValidatorsPack",
                ProjectUri = "SomeLocation/SomeFolder",
                AssemblyName = "Workers.exe",
                TargetExtension = "exe",
                Components =
                {
                    new RepoCat.Transmission.Models.ComponentManifest()
                    {
                        Name = "XmlValidator",
                        Description = "A thing that validates strings",
                        DocumentationUri = "http://google.com",
                        Tags = new List<string>() {"XML", "Schema", "Validity"},
                        Properties = new Dictionary<string, string>()
                        {
                            { "ComponentType", "Checker" },
                            { "Author", "Bjarmuz" }
                        }
                        
                    }
                },
                RepositoryName = this.testRepoOne.RepositoryName,
                OrganizationName = this.testRepoOne.OrganizationName
            };

            var insertedProject = await service.Upsert(prj).ConfigureAwait(false);
            prj.RepositoryName = this.testRepoTwo.RepositoryName;
            prj.OrganizationName = this.testRepoTwo.OrganizationName;
            var insertedProject2 = await service.Upsert(prj).ConfigureAwait(false);
            insertedProject.Id.Should().NotBe(insertedProject2.Id);

            ManifestQueryResult result = await GetResult(service, "findme");
            result.Projects.Count.Should().Be(0, "because the query does not match");
          

            result = await GetResult(service, "AcmeCorp");
            AssertProjectFoundBy("ProjectName", result, insertedProject);
          
            result = await GetResult(service, "Workers");
            AssertProjectFoundBy("AssemblyName", result, insertedProject);

            result = await GetResult(service, "SomeFolder");
            AssertProjectFoundBy("ProjectUri", result, insertedProject);

            result = await GetResult(service, "XmlValidator");
            AssertProjectFoundBy("ComponentName", result, insertedProject);

            result = await GetResult(service, "thing fixes strings");
            AssertProjectFoundBy("ComponentDescription", result, insertedProject);

            result = await GetResult(service, "google");
            AssertProjectFoundBy("ComponenDocsUri", result, insertedProject);

            result = await GetResult(service, "Validity");
            AssertProjectFoundBy("Tags", result, insertedProject);

            result = await GetResult(service, "Bjarmuz");
            AssertProjectFoundBy("PropertiesValue", result, insertedProject);

            result = await GetResult(service, "Checker");
            AssertProjectFoundBy("PropertiesValue", result, insertedProject);

           

        }

        [Test]
        public async Task TestGetProjectByQuery_DefaultRepo_VariousRepos_ShouldOnlyReturnProjectFromOneRepo()
        {
            RepositoryDatabase service = new RepositoryDatabase(Settings);
            ProjectInfo prj = new ProjectInfo()
            {
                ProjectName = "Project1",
                ProjectUri = "SomeLocation",
                RepositoryId = this.testRepoOne.Id,
                TargetExtension = "exe",
                Components = { new ComponentManifest()
                {
                    Tags = new List<string>(){"FindMe"}
                }}
            };

            //same tags, different repo
            await service.Upsert(prj).ConfigureAwait(false);
            prj.RepositoryId = this.testRepoTwo.Id;
            await service.Upsert(prj).ConfigureAwait(false);


            ProjectInfo prj3 = new ProjectInfo()
            {
                ProjectName = "Project3",
                ProjectUri = "SomeLocation",
                RepositoryId = this.testRepoOne.Id,
                TargetExtension = "dll",
                Components = { new ComponentManifest()
                {
                    Tags = new List<string>(){"ButNotMe"}
                }}
            };
            await service.Upsert(prj3).ConfigureAwait(false);

            ManifestQueryResult result = await service.GetCurrentProjects(this.testRepoOne.OrganizationName.ToUpperInvariant(), this.testRepoOne.RepositoryName.ToUpperInvariant(), "findme", false).ConfigureAwait(false);

            result.RepositoryName.Should().Be(this.testRepoOne.RepositoryName);
            result.Projects.Count.Should().Be(1, "because the other project is in a different repo");
            result.Projects[0].ProjectInfo.ProjectName.Should().Be("Project1");
        }
    }
}