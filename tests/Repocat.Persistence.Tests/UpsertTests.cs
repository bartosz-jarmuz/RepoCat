using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Microsoft.ApplicationInsights;
using MongoDB.Bson;
using MongoDB.Driver;
using NUnit.Framework;
using RepoCat.Persistence.Models;
using RepoCat.Persistence.Service;
using RepoCat.Portal.Mapping;
using RepoCat.RepositoryManagement.Service;
using RepositoryQueryParameter = RepoCat.RepositoryManagement.Service.RepositoryQueryParameter;

namespace Repocat.Persistence.Tests
{

    [TestFixture]
    public class UpsertTests
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

        private TelemetryClient telemetryClient;


       

        [SetUp]
        public void SetUp()
        {
            this.telemetryClient = TelemetryMock.InitializeMockTelemetryClient();
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
        public async Task TestUpsertRepo_CannotAddSameTwice_ShouldReturnTheSameId()
        {
            RepositoryDatabase service = new RepositoryDatabase(Settings);
            string organizationOne = Guid.NewGuid().ToString();
            string repoOne = Guid.NewGuid().ToString();
            RepositoryInfo repo = null;
            RepositoryInfo repo2 = null;

            Task t1 = Task.Run(async () =>
            {
                repo = await service.UpsertUpdate(new RepositoryInfo()
                    {
                        OrganizationName = organizationOne,
                        RepositoryName = repoOne
                    }
                ).ConfigureAwait(false);
                
            });
            Task t2 = Task.Run(async () =>
            {
                repo2 = await service.UpsertUpdate(new RepositoryInfo()
                    {
                        OrganizationName = organizationOne,
                        RepositoryName = repoOne
                    }
                ).ConfigureAwait(false);
            });
            //add both repos pretty much at the same time
            await Task.WhenAll(t1, t2);

            repo.Should().NotBeNull("Because it should have been created by the tasks invoke");

            repo.Id.Should().NotBe(ObjectId.Empty);
            repo.Id.Should().Be(repo2.Id, "because the same repository already exists");

        }


        [Test]
        public async Task TestUpsertRepo_SnapshotModeSetFromStart()
        {
            RepositoryDatabase service = new RepositoryDatabase(Settings);
            string organizationOne = MethodBase.GetCurrentMethod().Name;
            string repoOne = Guid.NewGuid().ToString();

            //add a new repository with snapshot mode
            RepositoryInfo repo = await service.UpsertUpdate(
                new RepositoryInfo()
                {
                    OrganizationName = organizationOne,
                    RepositoryName = repoOne,
                    RepositoryMode = RepositoryMode.Snapshot
                }).ConfigureAwait(false);
            repo.RepositoryMode.Should().Be(RepositoryMode.Snapshot);

        }

        [Test]
        public async Task TestUpsertRepo_ShouldReturnProperMode()
        {
            RepositoryDatabase service = new RepositoryDatabase(Settings);
            string organizationOne = MethodBase.GetCurrentMethod().Name;
            string repoOne = Guid.NewGuid().ToString();

            //first add a new repository to a new organization
            RepositoryInfo repo = await service.UpsertUpdate(
                new RepositoryInfo()
                {
                    OrganizationName = organizationOne,
                    RepositoryName = repoOne
                }).ConfigureAwait(false);
            repo.RepositoryMode.Should().Be(RepositoryMode.Default);

            //then add it again
            RepositoryInfo repo2 = await service.UpsertUpdate(new RepositoryInfo()
            {
                OrganizationName = organizationOne,
                RepositoryName = repoOne
            }).ConfigureAwait(false);
            repo2.RepositoryMode.Should().Be(RepositoryMode.Default);

            //then change the repo mode (ensure that change worked OK)
            repo.RepositoryMode = RepositoryMode.Snapshot;
            await service.UpsertReplace(repo).ConfigureAwait(false);
            repo = await service.GetRepository(organizationOne, repoOne);
            repo.RepositoryMode.Should().Be(RepositoryMode.Snapshot);

            //now perform upsert again and ensure that mode is not overwritten
            repo = await service.UpsertUpdate(new RepositoryInfo()
            {
                OrganizationName = organizationOne,
                RepositoryName = repoOne
            }).ConfigureAwait(false);
            repo.RepositoryMode.Should().Be(RepositoryMode.Snapshot);
            
            //even if added multiple times
            repo = await service.UpsertUpdate(new RepositoryInfo()
            {
                OrganizationName = organizationOne,
                RepositoryName = repoOne
            }).ConfigureAwait(false);
            repo.RepositoryMode.Should().Be(RepositoryMode.Snapshot);
        }

        [Test]
        public async Task TestUpsertProject_DefaultRepo_ShouldReturnUpdatedProjects()
        {
            RepositoryDatabase database = new RepositoryDatabase(Settings);
            var service = new RepositoryManagementService(database, new Mapper(MappingConfigurationFactory.Create()), this.telemetryClient);
            var allProjects = await service.GetAllCurrentProjects(new RepositoryQueryParameter(this.testRepoOne)).ConfigureAwait(false);
            allProjects.Projects.Count.Should().Be(0, "because there are no projects yet");

            await this.AddTwoProjectsWithSameRepoStamp(service).ConfigureAwait(false);

            ProjectInfo returnedPrj2 = await this.AddTwoProjectsWithSameRepoStamp(service).ConfigureAwait(false);

            //now add just one with new stamp and some changed properties (except for name and URI)
            RepoCat.Transmission.Models.ProjectInfo prj2Again = new RepoCat.Transmission.Models.ProjectInfo()
            {
                ProjectName = "Project2",
                ProjectUri = "SomeLocation",
                RepositoryInfo = new RepoCat.Transmission.Models.RepositoryInfo()
                {
                    RepositoryName = this.testRepoOne.RepositoryName,
                    OrganizationName = this.testRepoOne.OrganizationName,
                },
                
                TargetExtension = "exe",
                RepositoryStamp = "2.0",
                AssemblyName = "Project2AssName_NEW",
                Components =
                {
                    new RepoCat.Transmission.Models.ComponentManifest(new List<string>(){"Three", "Four"}, new Dictionary<string, string>()
                        {
                            {"KeyTwo","ValueTwo" }
                        }
                    )
                }
            };

            var returnedPrj2Again = await service.Upsert(prj2Again).ConfigureAwait(false);

            allProjects = await service.GetAllCurrentProjects(new RepositoryQueryParameter(this.testRepoOne)).ConfigureAwait(false);


            allProjects.Projects.Count.Should().Be(2, "because we don't return latest stamp only");
            allProjects.Projects.Should().Contain(x=>x.ProjectInfo.RepositoryStamp == "2.0");
            allProjects.Projects.Should().Contain(x=>x.ProjectInfo.RepositoryStamp == "1.0");

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
        public async Task TestUpsertProject_DefaultRepo_ShouldReturnTheSameIdButNewProperties()
        {
            RepositoryDatabase service = new RepositoryDatabase(Settings);
            ProjectInfo prj = new ProjectInfo()
            {
                ProjectName = "Project1",
                ProjectUri = "SomeLocation",
                RepositoryId = this.testRepoOne.Id,
                TargetExtension = "exe"
            };
            prj.Id.Should().Be(ObjectId.Empty);
            ProjectInfo returnedPrj = await service.Upsert(prj).ConfigureAwait(false);
            returnedPrj.Id.Should().NotBe(ObjectId.Empty);

            ProjectInfo prj2 = new ProjectInfo()
            {
                ProjectName = "Project1",
                ProjectUri = "SomeLocation",
                RepositoryId = this.testRepoOne.Id,
                TargetExtension = "dll"

            };

            ProjectInfo returnedPrj2 = await service.Upsert(prj2).ConfigureAwait(false);

            returnedPrj2.Id.Should().BeEquivalentTo(returnedPrj.Id, "because it's the same project");

            ProjectInfo attempt3 = await service.GetById(returnedPrj2.Id.ToString()).ConfigureAwait(false);
            attempt3.TargetExtension.Should().Be("dll", "because the project was updated");
        }

        [Test]
        public async Task TestUpsertProject_SnapshotRepo_ShouldOnlyReturnLastProjects()
        {
            RepositoryDatabase database = new RepositoryDatabase(Settings);
            var service = new RepositoryManagementService(database, new Mapper(MappingConfigurationFactory.Create()), this.telemetryClient);
            
            await SetSnapshotMode(database, this.testRepoOne).ConfigureAwait(false);

            var allProjects = await service.GetAllCurrentProjects(new RepositoryQueryParameter(this.testRepoOne)).ConfigureAwait(false);
            allProjects.Projects.Count.Should().Be(0, "because there are no projects yet");

            ProjectInfo returnedPrj2 = await this.AddTwoProjectsWithSameRepoStamp(service).ConfigureAwait(false);

            //now add just one with new stamp
            RepoCat.Transmission.Models.ProjectInfo prj2Again = new RepoCat.Transmission.Models.ProjectInfo()
            {
                ProjectName = "Project2",
                ProjectUri = "SomeLocation",
                RepositoryInfo = new RepoCat.Transmission.Models.RepositoryInfo()
                {
                    RepositoryName = this.testRepoOne.RepositoryName,
                    OrganizationName = this.testRepoOne.OrganizationName,
                },
                
                TargetExtension = "exe",
                RepositoryStamp = "2.0"
            };
            var returnedPrj2Again = await service.Upsert(prj2Again).ConfigureAwait(false);

            allProjects = await service.GetAllCurrentProjects(new RepositoryQueryParameter(this.testRepoOne)).ConfigureAwait(false);


            allProjects.Projects.Count.Should().Be(1, "because recently only added 1 project");
            allProjects.Projects.Should().OnlyContain(x => x.ProjectInfo.RepositoryStamp == "2.0");

            returnedPrj2Again.Id.Should().NotBe(returnedPrj2.Id);
            returnedPrj2Again.ProjectUri.Should().Be(returnedPrj2.ProjectUri);
            returnedPrj2Again.ProjectName.Should().Be(returnedPrj2.ProjectName);
            returnedPrj2Again.RepositoryId.Should().Be(returnedPrj2.RepositoryId);
        }

        private async Task<ProjectInfo> AddTwoProjectsWithSameRepoStamp(RepositoryManagementService service)
        {
            ManifestQueryResult allProjects;
//add two projects with same repo stamp
            RepoCat.Transmission.Models.ProjectInfo prj = new RepoCat.Transmission.Models.ProjectInfo()
            {
                ProjectName = "Project1",
                ProjectUri = "SomeLocation",
                RepositoryInfo = new RepoCat.Transmission.Models.RepositoryInfo()
                {
                    RepositoryName = this.testRepoOne.RepositoryName,
                    OrganizationName = this.testRepoOne.OrganizationName,
                },
                
                TargetExtension = "exe",
                RepositoryStamp = "1.0"
            };
            await service.Upsert(prj).ConfigureAwait(false);
            RepoCat.Transmission.Models.ProjectInfo prj2 = new RepoCat.Transmission.Models.ProjectInfo()
            {
                ProjectName = "Project2",
                ProjectUri = "SomeLocation",
                RepositoryInfo = new RepoCat.Transmission.Models.RepositoryInfo()
                {
                    RepositoryName = this.testRepoOne.RepositoryName,
                    OrganizationName = this.testRepoOne.OrganizationName,
                },
                
                TargetExtension = "exe",
                RepositoryStamp = "1.0",
                AssemblyName = "Project2AssName",
                Components =
                {
                    new RepoCat.Transmission.Models.ComponentManifest(new List<string>(){"One", "Two"}, new Dictionary<string, string>()
                    {
                        {"KeyOne","ValueOne" }
                    } 
                    )
                }
            };
            ProjectInfo returnedPrj2 = await service.Upsert(prj2).ConfigureAwait(false);

            allProjects = await service.GetAllCurrentProjects(new RepositoryQueryParameter(this.testRepoOne)).ConfigureAwait(false);

            allProjects.Projects.Count.Should().Be(2, "because we just added 2 projects with same stamp");
            allProjects.Projects.Should().OnlyContain(x => x.ProjectInfo.RepositoryStamp == "1.0");

            return returnedPrj2;
        }

        private static async Task SetSnapshotMode(RepositoryDatabase database, RepositoryInfo repository)
        {
            repository.RepositoryMode = RepositoryMode.Snapshot;

            await database.UpsertReplace(repository).ConfigureAwait(false);

            var repo = await database.GetRepositoryById(repository.Id).ConfigureAwait(false);
            repo.Should().BeEquivalentTo(repository);
            repo.RepositoryMode.Should().Be(RepositoryMode.Snapshot);
        }
    }
}