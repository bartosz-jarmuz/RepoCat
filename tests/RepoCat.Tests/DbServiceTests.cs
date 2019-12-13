using System;
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

namespace RepoCat.Tests
{
    [TestFixture]
    public class DbServiceTests
    {
        private static IRepoCatDbSettings settings = GetSettings();
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
            MongoClient client = new MongoClient(settings.ConnectionString);
            client.DropDatabase(settings.DatabaseName);

            RepositoryDatabase service = new RepositoryDatabase(settings);
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
        public async Task TestUpsertRepo_ShouldReturnTheSameId()
        {
            RepositoryDatabase service = new RepositoryDatabase(settings);
            string organizationOne = Guid.NewGuid().ToString();
            string repoOne = Guid.NewGuid().ToString();

            //first add a new repository to a new organization
            RepositoryInfo repo = await service.Upsert(organizationOne, repoOne).ConfigureAwait(false);
            repo.Id.Should().NotBe(ObjectId.Empty);

            //then add it again
            RepositoryInfo repo2 = await service.Upsert(organizationOne, repoOne).ConfigureAwait(false);

            repo2.Id.Should().Be(repo2.Id, "because the same repository already exists");
        }


        [Test]
        public async Task TestUpsertProject_DefaultRepo_ShouldReturnUpdatedProjects()
        {
            RepositoryDatabase database = new RepositoryDatabase(settings);
            var service = new RepositoryManagementService(database, new Mapper(MappingConfigurationFactory.Create()));
            var allProjects = await service.GetAllCurrentProjects(this.testRepoOne).ConfigureAwait(false);
            allProjects.Projects.Count.Should().Be(0, "because there are no projects yet");

            await this.AddTwoProjectsWithSameRepoStamp(service).ConfigureAwait(false);

            ProjectInfo returnedPrj2 = await this.AddTwoProjectsWithSameRepoStamp(service).ConfigureAwait(false);

            //now add just one with new stamp and some changed properties (except for name and URI)
            Transmission.Models.ProjectInfo prj2Again = new Transmission.Models.ProjectInfo()
            {
                ProjectName = "Project2",
                ProjectUri = "SomeLocation",
                RepositoryName = this.testRepoOne.RepositoryName,
                OrganizationName = this.testRepoOne.OrganizationName,
                TargetExtension = "exe",
                RepositoryStamp = "2.0",
                AssemblyName = "Project2AssName_NEW",
                Components =
                {
                    new Transmission.Models.ComponentManifest(new List<string>(){"Three", "Four"}, new Dictionary<string, string>()
                        {
                            {"KeyTwo","ValueTwo" }
                        }
                    )
                }
            };

            var returnedPrj2Again = await service.Upsert(prj2Again).ConfigureAwait(false);

            allProjects = await service.GetAllCurrentProjects(this.testRepoOne).ConfigureAwait(false);


            allProjects.Projects.Count.Should().Be(2, "because we don't return latest stamp only");
            allProjects.RepositoryStamp.Should().Be("2.0");

            returnedPrj2Again.Id.Should().Be(returnedPrj2.Id);
            returnedPrj2Again.ProjectUri.Should().Be(returnedPrj2.ProjectUri);
            returnedPrj2Again.ProjectName.Should().Be(returnedPrj2.ProjectName);
            returnedPrj2Again.RepositoryId.Should().Be(returnedPrj2.RepositoryId);

            var prj2ReturnedFromQuery = allProjects.Projects.Single(x => string.Equals(x.ProjectInfo.ProjectName, "Project2", StringComparison.Ordinal));

            prj2ReturnedFromQuery.ProjectInfo.Id.Should().Be(returnedPrj2Again.Id);
            prj2ReturnedFromQuery.ProjectInfo.RepositoryStamp.Should().Be("2.0");
            prj2ReturnedFromQuery.ProjectInfo.AssemblyName.Should().Be("Project2AssName_NEW");
            prj2ReturnedFromQuery.ProjectInfo.Components.Single().Tags.Count.Should().Be(2);
            prj2ReturnedFromQuery.ProjectInfo.Components.Single().Properties.Count.Should().Be(1);
            prj2ReturnedFromQuery.ProjectInfo.Components.Single().Properties["KeyTwo"].Should().Be("ValueTwo");
        }

        [Test]
        public async Task TestUpsertProject_SnapshotRepo_ShouldOnlyReturnLastProjects()
        {
            RepositoryDatabase database = new RepositoryDatabase(settings);
            var service = new RepositoryManagementService(database, new Mapper(MappingConfigurationFactory.Create()));
            
            await SetSnapshotMode(database, this.testRepoOne).ConfigureAwait(false);

            var allProjects = await service.GetAllCurrentProjects(this.testRepoOne).ConfigureAwait(false);
            allProjects.Projects.Count.Should().Be(0, "because there are no projects yet");

            ProjectInfo returnedPrj2 = await this.AddTwoProjectsWithSameRepoStamp(service).ConfigureAwait(false);

            //now add just one with new stamp
            Transmission.Models.ProjectInfo prj2Again = new Transmission.Models.ProjectInfo()
            {
                ProjectName = "Project2",
                ProjectUri = "SomeLocation",
                RepositoryName = this.testRepoOne.RepositoryName,
                OrganizationName = this.testRepoOne.OrganizationName,
                TargetExtension = "exe",
                RepositoryStamp = "2.0"
            };
            var returnedPrj2Again = await service.Upsert(prj2Again).ConfigureAwait(false);

            allProjects = await service.GetAllCurrentProjects(this.testRepoOne).ConfigureAwait(false);


            allProjects.Projects.Count.Should().Be(1, "because recently only added 1 project");
            allProjects.RepositoryStamp.Should().Be("2.0");
            returnedPrj2Again.Id.Should().NotBe(returnedPrj2.Id);
            returnedPrj2Again.ProjectUri.Should().Be(returnedPrj2.ProjectUri);
            returnedPrj2Again.ProjectName.Should().Be(returnedPrj2.ProjectName);
            returnedPrj2Again.RepositoryId.Should().Be(returnedPrj2.RepositoryId);
        }

        private async Task<ProjectInfo> AddTwoProjectsWithSameRepoStamp(RepositoryManagementService service)
        {
            ManifestQueryResult allProjects;
//add two projects with same repo stamp
            Transmission.Models.ProjectInfo prj = new Transmission.Models.ProjectInfo()
            {
                ProjectName = "Project1",
                ProjectUri = "SomeLocation",
                RepositoryName = this.testRepoOne.RepositoryName,
                OrganizationName = this.testRepoOne.OrganizationName,
                TargetExtension = "exe",
                RepositoryStamp = "1.0"
            };
            await service.Upsert(prj).ConfigureAwait(false);
            Transmission.Models.ProjectInfo prj2 = new Transmission.Models.ProjectInfo()
            {
                ProjectName = "Project2",
                ProjectUri = "SomeLocation",
                RepositoryName = this.testRepoOne.RepositoryName,
                OrganizationName = this.testRepoOne.OrganizationName,
                TargetExtension = "exe",
                RepositoryStamp = "1.0",
                AssemblyName = "Project2AssName",
                Components =
                {
                    new Transmission.Models.ComponentManifest(new List<string>(){"One", "Two"}, new Dictionary<string, string>()
                    {
                        {"KeyOne","ValueOne" }
                    } 
                    )
                }
            };
            ProjectInfo returnedPrj2 = await service.Upsert(prj2).ConfigureAwait(false);

            allProjects = await service.GetAllCurrentProjects(this.testRepoOne).ConfigureAwait(false);

            allProjects.Projects.Count.Should().Be(2, "because we just added 2 projects with same stamp");
            allProjects.RepositoryStamp.Should().Be("1.0");
            return returnedPrj2;
        }

        private static async Task SetSnapshotMode(RepositoryDatabase database, RepositoryInfo repository)
        {
            repository.RepositoryMode = RepositoryMode.Snapshot;

            await database.Replace(repository).ConfigureAwait(false);

            var repo = await database.GetRepositoryById(repository.Id).ConfigureAwait(false);
            repo.Should().BeEquivalentTo(repository);
            repo.RepositoryMode.Should().Be(RepositoryMode.Snapshot);
        }

        [Test]
        public async Task TestUpsertProject_ShouldReturnTheSameIdButNewProperties()
        {
            RepositoryDatabase service = new RepositoryDatabase(settings);
            ProjectInfo prj = new ProjectInfo()
            {
                ProjectName = "Project1",
                ProjectUri = "SomeLocation",
                RepositoryId = this.testRepoOne.Id,
                TargetExtension = "exe"
            };
             prj.Id.Should().Be(ObjectId.Empty);
            ProjectInfo  returnedPrj = await service.Upsert(prj).ConfigureAwait(false);
            returnedPrj.Id.Should().NotBe(ObjectId.Empty);

            ProjectInfo prj2 = new ProjectInfo()
            {
                ProjectName = "Project1",
                ProjectUri = "SomeLocation",
                RepositoryId = this.testRepoOne.Id,
                TargetExtension = "dll"

            };

            ProjectInfo returnedPrj2= await service.Upsert(prj2).ConfigureAwait(false);

            returnedPrj2.Id.Should().BeEquivalentTo(returnedPrj.Id, "because it's the same project");

            ProjectInfo attempt3 = await service.GetById(returnedPrj2.Id.ToString()).ConfigureAwait(false);
            attempt3.TargetExtension.Should().Be("dll", "because the project was updated");
        }




        [Test]
        public async Task TestGetProjectByQuery_VariousRepos_Tags()
        {
            RepositoryDatabase service = new RepositoryDatabase(settings);
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
                TargetExtension = "dll",
                Components = { new ComponentManifest()
                {
                Tags = new List<string>(){"FINDME"}
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

            result.Count.Should().Be(2);
            var returnedPrj1 = result.Single(x => x.ProjectInfo.ProjectName == "Project1");
            returnedPrj1.RepositoryInfo.RepositoryName.Should().Be("TestRepoOne");
            var returnedPrj2 = result.Single(x => x.ProjectInfo.ProjectName == "Project2");
            returnedPrj2.RepositoryInfo.RepositoryName.Should().Be("TestRepoTwo");
        }

        [Test]
        public async Task TestGetProjectByQuery_VariousRepos()
        {
            RepositoryDatabase service = new RepositoryDatabase(settings);
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
                TargetExtension = "dll",
                Components = { new ComponentManifest()
                {
                Tags = new List<string>(){"FINDME"}
            }}
            };
            await service.Upsert(prj2).ConfigureAwait(false);

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
            result.Projects.Count.Should().Be(1);
            result.Projects[0].ProjectInfo.ProjectName.Should().Be("Project1");
        }
    }
}