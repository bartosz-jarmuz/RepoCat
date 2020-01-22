using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using MongoDB.Bson;
using MongoDB.Driver;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using RepoCat.Persistence.Models;
using RepoCat.Persistence.Service;
using RepoCat.Portal.Mapping;
using RepoCat.RepositoryManagement.Service;
using RepositoryQueryParameter = RepoCat.RepositoryManagement.Service.RepositoryQueryParameter;

namespace Repocat.Persistence.Tests
{
    [TestFixture]
    public class GetProjectTests
    {
        private static readonly IRepoCatDbSettings Settings = TestHelpers.GetSettings();

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

            RepositoryDatabase database = new RepositoryDatabase(Settings);
            RepositoryInfo result = database.Create(this.testRepoOne);
            Assert.IsNotNull(result);
            RepositoryInfo result2 = database.Create(this.testRepoTwo);
            Assert.IsNotNull(result2);
        }

        [TearDown]
        public void TearDown()
        {
        }


        [Test]
        public async Task DefaultRepo_VariousRepos_DoNotFilterByRepo_ShouldReturnTwo()
        {
            RepositoryDatabase database = new RepositoryDatabase(Settings);

            ProjectInfo prj = new ProjectInfo()
            {
                ProjectName = "Project1",
                ProjectUri = "SomeLocation",
                RepositoryId = this.testRepoOne.Id,
                TargetExtension = "exe",
                Components =
                {
                    new ComponentManifest()
                    {
                        Tags = new List<string>() {"FindMe"}
                    }
                }
            };
            await database.Upsert(prj).ConfigureAwait(false);
            ProjectInfo prj2 = new ProjectInfo()
            {
                ProjectName = "Project2",
                ProjectUri = "SomeLocation",
                RepositoryId = this.testRepoTwo.Id,
                TargetExtension = "exe",
                Components =
                {
                    new ComponentManifest()
                    {
                        Tags = new List<string>() {"FindMe"}
                    }
                }
            };
            await database.Upsert(prj2).ConfigureAwait(false);

            ProjectInfo prj3 = new ProjectInfo()
            {
                ProjectName = "Project3",
                ProjectUri = "SomeLocation",
                RepositoryId = this.testRepoTwo.Id,
                TargetExtension = "dll",
                Components =
                {
                    new ComponentManifest()
                    {
                        Tags = new List<string>() {"ButNotMe"}
                    }
                }
            };
            await database.Upsert(prj3).ConfigureAwait(false);

            List<Project> result = (await database.GetProjectsByQuery("findme", false).ConfigureAwait(false)).ToList();

            result.Count.Should().Be(2, "because we are not filtering by repository");
            var returnedPrj1 = result.Single(x => x.ProjectInfo.ProjectName == "Project1");
            returnedPrj1.RepositoryInfo.RepositoryName.Should().Be("TestRepoOne");
            var returnedPrj2 = result.Single(x => x.ProjectInfo.ProjectName == "Project2");
            returnedPrj2.RepositoryInfo.RepositoryName.Should().Be("TestRepoTwo");
        }

        [Test]
        public async Task TestGetProject_TextFilter_ProjectName()
        {
            //arrange
            RepositoryDatabase database = new RepositoryDatabase(Settings);
            var service = new RepositoryManagementService(database, new Mapper(MappingConfigurationFactory.Create()), TelemetryMock.InitializeMockTelemetryClient());
            var prj = GetEmptyProject(this.testRepoOne);
            prj.ProjectName = "PinkPanther.ValidatorsPack";
            ProjectInfo insertedProject = await service.Upsert(prj).ConfigureAwait(false);
            await this.AddMoreProjectsToEnsureNotTooMuchIsReturned(prj, service, insertedProject);

            //act
            var result = await this.GetResultFromRepoOne(service, "PinkPanther");

            //assert
            this.AssertOneProjectFoundBy("ProjectName", result, insertedProject);
        }

        [Test]
        public async Task TestGetProject_TextFilter_ProjectDescription()
        {
            //arrange
            RepositoryDatabase database = new RepositoryDatabase(Settings);
            var service = new RepositoryManagementService(database, new Mapper(MappingConfigurationFactory.Create()), TelemetryMock.InitializeMockTelemetryClient());
            var prj = GetEmptyProject(this.testRepoOne);
            prj.ProjectDescription= "A general overview of a project functionality";
            ProjectInfo insertedProject = await service.Upsert(prj).ConfigureAwait(false);
            await this.AddMoreProjectsToEnsureNotTooMuchIsReturned(prj, service, insertedProject);

            //act
            var result = await this.GetResultFromRepoOne(service, "functionality");

            //assert
            this.AssertOneProjectFoundBy("ProjectDescription", result, insertedProject);
        }

        [Test]
        public async Task TestGetProject_TextFilter_NamePart()
        {
            //arrange
            RepositoryDatabase database = new RepositoryDatabase(Settings);
            var service = new RepositoryManagementService(database, new Mapper(MappingConfigurationFactory.Create()), TelemetryMock.InitializeMockTelemetryClient());
            var prj = GetEmptyProject(this.testRepoOne);
            prj.ProjectName = "PinkPanther.ValidatorsPack";
            ProjectInfo insertedProject = await service.Upsert(prj).ConfigureAwait(false);
            await this.AddMoreProjectsToEnsureNotTooMuchIsReturned(prj, service, insertedProject);
            //act
            var result = await this.GetResultFromRepoOne(service, "PinkPanther");
            //assert
            this.AssertOneProjectFoundBy("Name Part", result, insertedProject);
        }


        [Test]
        public async Task TestGetProject_TextFilter_ProjectTags()
        {
            //arrange
            RepositoryDatabase database = new RepositoryDatabase(Settings);
            var service = new RepositoryManagementService(database, new Mapper(MappingConfigurationFactory.Create()), TelemetryMock.InitializeMockTelemetryClient());
            var prj = GetEmptyProject(this.testRepoOne);
            prj.Tags.Add("ProjectTagOne");

            ProjectInfo insertedProject = await service.Upsert(prj).ConfigureAwait(false);
            await this.AddMoreProjectsToEnsureNotTooMuchIsReturned(prj, service, insertedProject);

            //act
            var result = await this.GetResultFromRepoOne(service, "ProjectTagOne");

            //assert
            this.AssertOneProjectFoundBy("ProjectTags", result, insertedProject);
        }

        [Test]
        public async Task TestGetProject_TextFilter_ProjectProperties()
        {
            //arrange
            RepositoryDatabase database = new RepositoryDatabase(Settings);
            var service = new RepositoryManagementService(database, new Mapper(MappingConfigurationFactory.Create()), TelemetryMock.InitializeMockTelemetryClient());
            var prj = GetEmptyProject(this.testRepoOne);
            prj.Properties.Add("ProjectPropertyOneKey", "ProjectPropertyOneValue");

            ProjectInfo insertedProject = await service.Upsert(prj).ConfigureAwait(false);
            await this.AddMoreProjectsToEnsureNotTooMuchIsReturned(prj, service, insertedProject);
            //act
            ManifestQueryResult result = await this.GetResultFromRepoOne(service, "ProjectPropertyOneValue");

            //assert
            this.AssertOneProjectFoundBy("ProjectProperties", result, insertedProject);
        }

        [Test]
        public async Task TestGetProject_TextFilter_NoMatch()
        {
            //arrange
            RepositoryDatabase database = new RepositoryDatabase(Settings);
            var service = new RepositoryManagementService(database, new Mapper(MappingConfigurationFactory.Create()), TelemetryMock.InitializeMockTelemetryClient());
            var prj = GetEmptyProject(this.testRepoOne);
            prj.ProjectName = "PinkPanther.ValidatorsPack";
            ProjectInfo insertedProject = await service.Upsert(prj).ConfigureAwait(false);
            await this.AddMoreProjectsToEnsureNotTooMuchIsReturned(prj, service, insertedProject);
            //act
            ManifestQueryResult result = await this.GetResultFromRepoOne(service, "findme");

            //assert
            result.Projects.Count.Should().Be(0, "because the query does not match");
        }

        [Test]
        public async Task TestGetProject_TextFilter_AssemblyName()
        {
            //arrange
            RepositoryDatabase database = new RepositoryDatabase(Settings);
            var service = new RepositoryManagementService(database, new Mapper(MappingConfigurationFactory.Create()), TelemetryMock.InitializeMockTelemetryClient());
            var prj = GetEmptyProject(this.testRepoOne);
            prj.AssemblyName = "Workers.exe";
            ProjectInfo insertedProject = await service.Upsert(prj).ConfigureAwait(false);
            await this.AddMoreProjectsToEnsureNotTooMuchIsReturned(prj, service, insertedProject);
            //act
            var result = await this.GetResultFromRepoOne(service, "Workers");
            //assert
            this.AssertOneProjectFoundBy("AssemblyName", result, insertedProject);
        }


        [Test]
        public async Task TestGetProject_TextFilter_ProjectUri() //not sure if want to search by this, but ok for now
        {
            //arrange
            RepositoryDatabase database = new RepositoryDatabase(Settings);
            var service = new RepositoryManagementService(database, new Mapper(MappingConfigurationFactory.Create()), TelemetryMock.InitializeMockTelemetryClient());
            var prj = GetEmptyProject(this.testRepoOne);
            prj.ProjectUri = "SomeLocation/SomeFolder";
            ProjectInfo insertedProject = await service.Upsert(prj).ConfigureAwait(false);
            await this.AddMoreProjectsToEnsureNotTooMuchIsReturned(prj, service, insertedProject);
            //act
            var result = await this.GetResultFromRepoOne(service, "SomeFolder");
            //assert
            this.AssertOneProjectFoundBy("ProjectUri", result, insertedProject);
        }

        [Test]
        public async Task TestGetProject_TextFilter_ComponentName()
        {
            //arrange
            RepositoryDatabase database = new RepositoryDatabase(Settings);
            var service = new RepositoryManagementService(database, new Mapper(MappingConfigurationFactory.Create()), TelemetryMock.InitializeMockTelemetryClient());
            var prj = GetEmptyProject(this.testRepoOne);
            prj.Components.Add(new RepoCat.Transmission.Models.ComponentManifest()
            {
                Name = "XmlValidator",
            });

            ProjectInfo insertedProject = await service.Upsert(prj).ConfigureAwait(false);
            await this.AddMoreProjectsToEnsureNotTooMuchIsReturned(prj, service, insertedProject);
            //act
            var result = await this.GetResultFromRepoOne(service, "XmlValidator");
            //assert
            this.AssertOneProjectFoundBy("ComponentName", result, insertedProject);
        }


        [Test]
        public async Task TestGetProject_TextFilter_ComponentDescription()
        {
            //arrange
            RepositoryDatabase database = new RepositoryDatabase(Settings);
            var service = new RepositoryManagementService(database, new Mapper(MappingConfigurationFactory.Create()), TelemetryMock.InitializeMockTelemetryClient());
            var prj = GetEmptyProject(this.testRepoOne);
            prj.Components.Add(new RepoCat.Transmission.Models.ComponentManifest()
            {
                Description = "A thing that validates strings",
            });

            ProjectInfo insertedProject = await service.Upsert(prj).ConfigureAwait(false);
            await this.AddMoreProjectsToEnsureNotTooMuchIsReturned(prj, service, insertedProject);
            //act
            var result = await this.GetResultFromRepoOne(service, "fixes strings");
            //assert
            this.AssertOneProjectFoundBy("ComponentDescription", result, insertedProject);
        }

        [Test]
        public async Task TestGetProject_TextFilter_ComponentDocsUri()
        {
            //arrange
            RepositoryDatabase database = new RepositoryDatabase(Settings);
            var service = new RepositoryManagementService(database, new Mapper(MappingConfigurationFactory.Create()), TelemetryMock.InitializeMockTelemetryClient());
            var prj = GetEmptyProject(this.testRepoOne);
            prj.Components.Add(new RepoCat.Transmission.Models.ComponentManifest()
            {
                DocumentationUri = "http://google.com",
            });

            ProjectInfo insertedProject = await service.Upsert(prj).ConfigureAwait(false);
            await this.AddMoreProjectsToEnsureNotTooMuchIsReturned(prj, service, insertedProject);
            //act
            var result = await this.GetResultFromRepoOne(service, "google");
            //assert
            this.AssertOneProjectFoundBy("ComponenDocsUri", result, insertedProject);
        }

        [Test]
        public async Task TestGetProject_TextFilter_ComponentTags()
        {
            //arrange
            RepositoryDatabase database = new RepositoryDatabase(Settings);
            var service = new RepositoryManagementService(database, new Mapper(MappingConfigurationFactory.Create()), TelemetryMock.InitializeMockTelemetryClient());
            var prj = GetEmptyProject(this.testRepoOne);
            prj.Components.Add(new RepoCat.Transmission.Models.ComponentManifest()
            {
                Tags = {"XML", "Schema", "Validity"},
            });

            ProjectInfo insertedProject = await service.Upsert(prj).ConfigureAwait(false);
            await this.AddMoreProjectsToEnsureNotTooMuchIsReturned(prj, service, insertedProject);
            //act
            var result = await this.GetResultFromRepoOne(service, "Validity");
            //assert
            this.AssertOneProjectFoundBy("Tags", result, insertedProject);
        }

        [Test]
        public async Task TestGetProject_TextFilter_ComponentProperties()
        {
            //arrange
            RepositoryDatabase database = new RepositoryDatabase(Settings);
            var service = new RepositoryManagementService(database, new Mapper(MappingConfigurationFactory.Create()), TelemetryMock.InitializeMockTelemetryClient());
            var prj = GetEmptyProject(this.testRepoOne);
            prj.Components.Add(new RepoCat.Transmission.Models.ComponentManifest()
            {
                Properties ={{"ComponentType", "Checker"},
                    {"Author", "Bjarmuz"}
                },
            });

            ProjectInfo insertedProject = await service.Upsert(prj).ConfigureAwait(false);
            await this.AddMoreProjectsToEnsureNotTooMuchIsReturned(prj, service, insertedProject);
            //act
            var result = await this.GetResultFromRepoOne(service, "Bjarmuz");
            //assert
            this.AssertOneProjectFoundBy("PropertiesValue", result, insertedProject);

            result = await this.GetResultFromRepoOne(service, "Checker");
            this.AssertOneProjectFoundBy("PropertiesValue", result, insertedProject);
        }


        [Test]
        public async Task SnapshotRepo_ShouldReturnOnlyLatest()
        {
            //arrange
            RepositoryDatabase database = new RepositoryDatabase(Settings);
            var service = new RepositoryManagementService(database, new Mapper(MappingConfigurationFactory.Create()),
                TelemetryMock.InitializeMockTelemetryClient());

            RepositoryInfo repo = await database.UpsertUpdate(
                new RepositoryInfo()
                {
                    OrganizationName = $"Organization_{TestHelpers.GetMethodName()}",
                    RepositoryName = $"Repository_{TestHelpers.GetMethodName()}",
                    RepositoryMode = RepositoryMode.Snapshot
                }).ConfigureAwait(false);
            //add project 1V1
            ProjectInfo prj = new ProjectInfo()
            {
                ProjectName = "Banana Tool",
                RepositoryId = repo.Id,
                RepositoryStamp = "1.0",
                Components =
                {
                    new ComponentManifest()
                    {
                        Tags = new List<string>() {"TagFromV1"}
                    }
                }
            };
            ProjectInfo bananaToolV1 = await database.Upsert(prj).ConfigureAwait(false);
            //add v2 of the same project
            prj = new ProjectInfo()
            {
                ProjectName = "Banana Tool",
                RepositoryId = repo.Id,
                RepositoryStamp = "2.0",
                Components =
                {
                    new ComponentManifest()
                    {
                        Tags = new List<string>() {"VersionTwoTag"}
                    }
                }
            };
            ProjectInfo bananaToolV2 = await database.Upsert(prj).ConfigureAwait(false);

            //add different project that could be found with the same query to v1 (banana tag)
            prj = new ProjectInfo()
            {
                ProjectName = "Pineapple Tool",
                RepositoryId = repo.Id,
                RepositoryStamp = "1.0",
                Components =
                {
                    new ComponentManifest()
                    {
                        Tags = new List<string>() {"Banana"}
                    }
                }
            };
            ProjectInfo pineAppleTool = await database.Upsert(prj).ConfigureAwait(false);
            //add a project that does not match query but sits in latest version
            prj = new ProjectInfo()
            {
                ProjectName = "Apple Tool",
                RepositoryId = repo.Id,
                RepositoryStamp = "2.0",
                Components =
                {
                    new ComponentManifest()
                    {
                        Tags = new List<string>() {"Apple"}
                    }
                }
            };
            ProjectInfo appleTool = await database.Upsert(prj).ConfigureAwait(false);


            //act
            var byQueryResult = await service.GetCurrentProjects(new RepositoryQueryParameter(repo), "Banana", false)
                .ConfigureAwait(false);
            var allProjectsResult = await service.GetAllCurrentProjects(new RepositoryQueryParameter(repo))
                .ConfigureAwait(false);

            //assert by query
            byQueryResult.Projects.Count.Should().Be(1, "because only one project in V2 matches this query");
            byQueryResult.Projects.Single().ProjectInfo.Should().BeEquivalentTo(bananaToolV2);

            //assert get all
            allProjectsResult.Projects.Count.Should().Be(2, "because there are 2 projects");
            allProjectsResult.Projects.Should().OnlyHaveUniqueItems();
            Assert.That(() => allProjectsResult.Projects.Any(x => x.ProjectInfo.Id == appleTool.Id));
            Assert.That(() => allProjectsResult.Projects.Any(x => x.ProjectInfo.Id == bananaToolV2.Id));
        }

        [Test]
        public async Task DefaultRepo_ShouldReturnAllMatching()
        {
            //arrange
            RepositoryDatabase database = new RepositoryDatabase(Settings);
            var service = new RepositoryManagementService(database, new Mapper(MappingConfigurationFactory.Create()),
                TelemetryMock.InitializeMockTelemetryClient());

            RepositoryInfo repo = await database.UpsertUpdate(
                new RepositoryInfo()
                {
                    OrganizationName = $"Organization_{TestHelpers.GetMethodName()}",
                    RepositoryName = $"Repository_{TestHelpers.GetMethodName()}",
                }).ConfigureAwait(false);
            //add project 1V1
            ProjectInfo prj = new ProjectInfo()
            {
                ProjectName = "Banana Tool",
                ProjectUri = "LocationOne",
                RepositoryId = repo.Id,
                RepositoryStamp = "1.0",
                Components =
                {
                    new ComponentManifest()
                    {
                        Tags = new List<string>() {"TagFromV1"}
                    }
                }
            };
            ProjectInfo bananaToolV1 = await database.Upsert(prj).ConfigureAwait(false);

            //add v1.1 of the same project

            prj = new ProjectInfo()
            {
                ProjectName = "Banana Tool",
                ProjectUri = "LocationOne",
                RepositoryId = repo.Id,
                RepositoryStamp = "1.1",
                Components =
                {
                    new ComponentManifest()
                    {
                        Tags = new List<string>() {"TagFromV1"}
                    }
                }
            };
            ProjectInfo bananaToolV11 = await database.Upsert(prj).ConfigureAwait(false);
            //add similar project in different location (so, a different project for default repo)
            prj = new ProjectInfo()
            {
                ProjectName = "Banana Tool",
                ProjectUri = "LocationTwo",
                RepositoryId = repo.Id,
                RepositoryStamp = "2.0",
                Components =
                {
                    new ComponentManifest()
                    {
                        Tags = new List<string>() {"VersionTwoTag"}
                    }
                }
            };
            ProjectInfo bananaToolV2 = await database.Upsert(prj).ConfigureAwait(false);

            //add different project that could be found with the same query to v1 (banana tag)
            prj = new ProjectInfo()
            {
                ProjectName = "Pineapple Tool",
                RepositoryId = repo.Id,
                RepositoryStamp = "1.0",
                Components =
                {
                    new ComponentManifest()
                    {
                        Tags = new List<string>() {"Banana"}
                    }
                }
            };
            ProjectInfo pineAppleTool = await database.Upsert(prj).ConfigureAwait(false);
            //add a project that does not match query but sits in latest version
            prj = new ProjectInfo()
            {
                ProjectName = "Apple Tool",
                RepositoryId = repo.Id,
                RepositoryStamp = "2.0",
                Components =
                {
                    new ComponentManifest()
                    {
                        Tags = new List<string>() {"Apple"}
                    }
                }
            };
            ProjectInfo appleTool = await database.Upsert(prj).ConfigureAwait(false);


            //act
            var byQueryResult = await service.GetCurrentProjects(new RepositoryQueryParameter(repo), "Banana", false)
                .ConfigureAwait(false);
            var allProjectsResult = await service.GetAllCurrentProjects(new RepositoryQueryParameter(repo))
                .ConfigureAwait(false);

            //assert by query
            byQueryResult.Projects.Count.Should().Be(3, "because 3 projects this query");
            byQueryResult.Projects.Should().OnlyHaveUniqueItems();

            Assert.That(() => byQueryResult.Projects.Any(x => x.ProjectInfo.Id == bananaToolV11.Id));
            Assert.That(() => byQueryResult.Projects.Any(x => x.ProjectInfo.Id == bananaToolV2.Id));
            Assert.That(() => byQueryResult.Projects.Any(x => x.ProjectInfo.Id == pineAppleTool.Id));


            //assert get all
            allProjectsResult.Projects.Count.Should().Be(4, "because there are 4 projects");
            allProjectsResult.Projects.Should().OnlyHaveUniqueItems();


            Assert.That(() => allProjectsResult.Projects.Any(x => x.ProjectInfo.Id == bananaToolV11.Id));
            Assert.That(() => allProjectsResult.Projects.Any(x => x.ProjectInfo.Id == bananaToolV2.Id));
            Assert.That(() => allProjectsResult.Projects.Any(x => x.ProjectInfo.Id == pineAppleTool.Id));
            Assert.That(() => allProjectsResult.Projects.Any(x => x.ProjectInfo.Id == appleTool.Id));
        }


        [Test]
        public async Task MultipleRepos_ShoulReturnProjectsFromBoth()
        {
            RepositoryDatabase database = new RepositoryDatabase(Settings);
            var service = new RepositoryManagementService(database, new Mapper(MappingConfigurationFactory.Create()),
                TelemetryMock.InitializeMockTelemetryClient());

            ProjectInfo prj = new ProjectInfo()
            {
                ProjectName = "Project1",
                ProjectUri = "SomeLocation",
                RepositoryId = this.testRepoOne.Id,
                TargetExtension = "exe",
                Components =
                {
                    new ComponentManifest()
                    {
                        Tags = new List<string>() {"FindMe"}
                    }
                }
            };

            //same tags, different repo
            await database.Upsert(prj).ConfigureAwait(false);
            prj.RepositoryId = this.testRepoTwo.Id;
            await database.Upsert(prj).ConfigureAwait(false);


            ProjectInfo prj3 = new ProjectInfo()
            {
                ProjectName = "Project3",
                ProjectUri = "SomeLocation",
                RepositoryId = this.testRepoOne.Id,
                TargetExtension = "dll",
                Components =
                {
                    new ComponentManifest()
                    {
                        Tags = new List<string>() {"ButNotMe"}
                    }
                }
            };
            await database.Upsert(prj3).ConfigureAwait(false);

            ManifestQueryResult result = await service.GetCurrentProjects(
                new RepositoryQueryParameter(this.testRepoOne.OrganizationName.ToUpperInvariant(),
                    this.testRepoOne.RepositoryName.ToUpperInvariant()),
                "findme", false).ConfigureAwait(false);
            result.Projects.Should()
                .OnlyContain(x => x.RepositoryInfo.RepositoryName == this.testRepoOne.RepositoryName);
            result.Projects.Count.Should().Be(1, "because the other project is in a different repo");
            result.Projects[0].ProjectInfo.ProjectName.Should().Be("Project1");
        }

        [Test]
        public async Task MultipleRepos_ShouldReturnProjectsFromCorrectSnapshots()
        {
            //getting projects is available from multiple repos
            //these repos can have different modes
            //when returning the projects, ensure that you only get the latest matching ones from snapshot repos 
            //bear in mind that different repos can have different latest version stamp, so treat the filtering separately.


            //seed snapshot repo 1
            //arrange
            RepositoryDatabase database = new RepositoryDatabase(Settings);
            var service = new RepositoryManagementService(database, new Mapper(MappingConfigurationFactory.Create()),
                TelemetryMock.InitializeMockTelemetryClient());

            //one project matches banana by query (and has v2, not v1)
            SeedResult snapshotRepo1Seed = await SeedSnapshotRepoOneForMultiRepoQuery(database);

            //one project matches banana by query (and has v6, not v5)
            SeedResult snapshotRepo2Seed = await SeedSnapshotRepoTwoForMultiRepoQuery(database);

            //two projects banana by query
            SeedResult defaultRepo1Seed = await SeedDefaultRepoOneForMultiRepoQuery(database);

            var parameters = new List<RepositoryQueryParameter>()
            {
                new RepositoryQueryParameter(snapshotRepo1Seed.Repository.OrganizationName,
                    snapshotRepo1Seed.Repository.RepositoryName),
                new RepositoryQueryParameter(snapshotRepo2Seed.Repository.OrganizationName,
                    snapshotRepo2Seed.Repository.RepositoryName),
                new RepositoryQueryParameter(defaultRepo1Seed.Repository.OrganizationName,
                    defaultRepo1Seed.Repository.RepositoryName)
            };
            //act
            var byQueryResult = await service.GetCurrentProjects(parameters, "Banana", false).ConfigureAwait(false);


            var allProjectsResult = await service.GetAllCurrentProjects(parameters).ConfigureAwait(false);

            //assert by query
            byQueryResult.Projects.Count.Should().Be(4, "because only one project in V2 matches this query");
            byQueryResult.Projects.Should().Contain(x => x.RepositoryInfo.Id == snapshotRepo1Seed.Repository.Id);
            byQueryResult.Projects.Should().Contain(x => x.RepositoryInfo.Id == snapshotRepo2Seed.Repository.Id);
            byQueryResult.Projects.Should().Contain(x => x.RepositoryInfo.Id == defaultRepo1Seed.Repository.Id);

            var projectsFromSnapshotRepo1 = byQueryResult.Projects
                .Where(x => x.RepositoryInfo.Id == snapshotRepo1Seed.Repository.Id).ToList();
            var projectsFromSnapshotRepo2 = byQueryResult.Projects
                .Where(x => x.RepositoryInfo.Id == snapshotRepo2Seed.Repository.Id).ToList();
            var projectsDefaultFromRepo1 = byQueryResult.Projects
                .Where(x => x.RepositoryInfo.Id == defaultRepo1Seed.Repository.Id).ToList();

            projectsFromSnapshotRepo1.Single().ProjectInfo.RepositoryStamp.Should().Be("2.0");
            projectsFromSnapshotRepo1.Single().ProjectInfo.ProjectName.Should().Be("Banana Tool");

            projectsFromSnapshotRepo2.Single().ProjectInfo.RepositoryStamp.Should().Be("6.0");
            projectsFromSnapshotRepo2.Single().ProjectInfo.ProjectName.Should().Be("Banana Tool");

            Assert.AreEqual(2, projectsDefaultFromRepo1.Count());
            Assert.AreEqual(1,
                projectsDefaultFromRepo1.Count(x =>
                    x.ProjectInfo.ProjectName == "Banana Tool" && x.ProjectInfo.ProjectUri == "LocationOne"));
            Assert.AreEqual(1,
                projectsDefaultFromRepo1.Count(x =>
                    x.ProjectInfo.ProjectName == "Banana Tool" && x.ProjectInfo.ProjectUri == "LocationTwo"));


            foreach (ProjectInfo matchingProject in snapshotRepo1Seed.MatchingProjects
                .Concat(snapshotRepo2Seed.MatchingProjects).Concat(defaultRepo1Seed.MatchingProjects))
            {
                Assert.That(() => byQueryResult.Projects.Any(x => x.ProjectInfo.Id == matchingProject.Id));
            }

            var result = await database.GetSummary().ConfigureAwait(false);
        }

        [Test]
        public async Task TestWeights_Project_ShouldReturnOrdered()
        {
            //arrange
            RepositoryDatabase database = new RepositoryDatabase(Settings);
            var service = new RepositoryManagementService(database, new Mapper(MappingConfigurationFactory.Create()),
                TelemetryMock.InitializeMockTelemetryClient());

            var prj2 = GetEmptyProject(this.testRepoOne, "Second");
            prj2.ProjectDescription = "project description";
            await service.Upsert(prj2).ConfigureAwait(false);

            var prj = GetEmptyProject(this.testRepoOne, "First");
            prj.Tags.Add("Tag");

            await service.Upsert(prj).ConfigureAwait(false);

            for (int i = 0; i < 500; i++)
            {
                var result = await this.GetResultFromRepoOne(service, "project description tag").ConfigureAwait(false);
                Assert.AreEqual("First", result.Projects[0].ProjectInfo.ProjectName, $"Failed at attempt [{i}]");
                Assert.AreEqual("Second", result.Projects[1].ProjectInfo.ProjectName, $"Failed at attempt [{i}]");
            }
        }

        [Test]
        public async Task TestWeights_Components_ShouldReturnOrdered()
        {
            //arrange
            RepositoryDatabase database = new RepositoryDatabase(Settings);
            var service = new RepositoryManagementService(database, new Mapper(MappingConfigurationFactory.Create()),
                TelemetryMock.InitializeMockTelemetryClient());

            var prj2 = GetEmptyProject(this.testRepoOne, "Second");
            prj2.Components.Add(new RepoCat.Transmission.Models.ComponentManifest(){Description = "ComponentManifest description" });
            await service.Upsert(prj2).ConfigureAwait(false);

            var prj = GetEmptyProject(this.testRepoOne, "First");
            prj.Components.Add(new RepoCat.Transmission.Models.ComponentManifest(new List<string>(){"tag"},new Dictionary<string, string>()));

            await service.Upsert(prj).ConfigureAwait(false);

            for (int i = 0; i < 500; i++)
            {
                var result = await this.GetResultFromRepoOne(service, "ComponentManifest description tag").ConfigureAwait(false);
                Assert.AreEqual("First", result.Projects[0].ProjectInfo.ProjectName, $"Failed at attempt [{i}]");
                Assert.AreEqual("Second", result.Projects[1].ProjectInfo.ProjectName, $"Failed at attempt [{i}]");
            }
            
        }


        private static async Task<SeedResult> SeedDefaultRepoOneForMultiRepoQuery(RepositoryDatabase database)
        {
            RepositoryInfo defaultRepo1 = await database.UpsertUpdate(
                new RepositoryInfo()
                {
                    OrganizationName = $"Organization_{TestHelpers.GetMethodName()}",
                    RepositoryName = $"Repository_{TestHelpers.GetMethodName()}",
                }).ConfigureAwait(false);
            //add project 1V1
            ProjectInfo prj = new ProjectInfo()
            {
                ProjectName = "Banana Tool",
                ProjectUri = "LocationOne",
                RepositoryId = defaultRepo1.Id,
                RepositoryStamp = "1.0",
                Components =
                {
                    new ComponentManifest()
                    {
                        Tags = new List<string>() {"TagFromV1"}
                    }
                }
            };
            var bananaTool = await database.Upsert(prj).ConfigureAwait(false);

            //add similar but different project (so, default repo treats them as separate entities, regardless of stamps)
            prj = new ProjectInfo()
            {
                ProjectName = "Banana Tool",
                ProjectUri = "LocationTwo",
                RepositoryId = defaultRepo1.Id,
                RepositoryStamp = "10.0",
                Components =
                {
                    new ComponentManifest()
                    {
                        Tags = new List<string>() {"TagFromV1"}
                    }
                }
            };
            var similarBananTool = await database.Upsert(prj).ConfigureAwait(false);

            return new SeedResult()
            {
                Repository = defaultRepo1,
                MatchingProjects = new List<ProjectInfo>() {bananaTool, similarBananTool}
            };
        }

        private class SeedResult
        {
            public RepositoryInfo Repository { get; set; }
            public List<ProjectInfo> MatchingProjects { get; set; }
        }

        [Test]
        public async Task DefaultRepo_VariousRepos_ShouldOnlyReturnProjectFromOneRepo()
        {
            RepositoryDatabase database = new RepositoryDatabase(Settings);
            var service = new RepositoryManagementService(database, new Mapper(MappingConfigurationFactory.Create()),
                TelemetryMock.InitializeMockTelemetryClient());

            ProjectInfo prj = new ProjectInfo()
            {
                ProjectName = "Project1",
                ProjectUri = "SomeLocation",
                RepositoryId = this.testRepoOne.Id,
                TargetExtension = "exe",
                Components =
                {
                    new ComponentManifest()
                    {
                        Tags = new List<string>() {"FindMe"}
                    }
                }
            };

            //same tags, different repo
            await database.Upsert(prj).ConfigureAwait(false);
            prj.RepositoryId = this.testRepoTwo.Id;
            await database.Upsert(prj).ConfigureAwait(false);


            ProjectInfo prj3 = new ProjectInfo()
            {
                ProjectName = "Project3",
                ProjectUri = "SomeLocation",
                RepositoryId = this.testRepoOne.Id,
                TargetExtension = "dll",
                Components =
                {
                    new ComponentManifest()
                    {
                        Tags = new List<string>() {"ButNotMe"}
                    }
                }
            };
            await database.Upsert(prj3).ConfigureAwait(false);

            ManifestQueryResult result = await service
                .GetCurrentProjects(
                    new RepositoryQueryParameter(this.testRepoOne.OrganizationName.ToUpperInvariant(),
                        this.testRepoOne.RepositoryName.ToUpperInvariant()), "findme", false).ConfigureAwait(false);

            result.Projects.Should()
                .OnlyContain(x => x.RepositoryInfo.RepositoryName == this.testRepoOne.RepositoryName);
            result.Projects.Count.Should().Be(1, "because the other project is in a different repo");
            result.Projects[0].ProjectInfo.ProjectName.Should().Be("Project1");
        }

        private static async Task<SeedResult> SeedSnapshotRepoOneForMultiRepoQuery(RepositoryDatabase database)
        {
            RepositoryInfo snapshotRepo1 = await database.UpsertUpdate(
                new RepositoryInfo()
                {
                    OrganizationName = $"Organization_{TestHelpers.GetMethodName()}",
                    RepositoryName = $"Repository_{TestHelpers.GetMethodName()}",
                    RepositoryMode = RepositoryMode.Snapshot
                }).ConfigureAwait(false);
            //add project 1V1
            ProjectInfo prj = new ProjectInfo()
            {
                ProjectName = "Banana Tool",
                RepositoryId = snapshotRepo1.Id,
                RepositoryStamp = "1.0",
                Components =
                {
                    new ComponentManifest()
                    {
                        Tags = new List<string>() {"TagFromV1"}
                    }
                }
            };
            await database.Upsert(prj).ConfigureAwait(false);
            //add v2 of the same project
            prj = new ProjectInfo()
            {
                ProjectName = "Banana Tool",
                RepositoryId = snapshotRepo1.Id,
                RepositoryStamp = "2.0",
                Components =
                {
                    new ComponentManifest()
                    {
                        Tags = new List<string>() {"VersionTwoTag"}
                    }
                }
            };
            var bananaToolV2 = await database.Upsert(prj).ConfigureAwait(false);

            //add different project that could be found with the same query to v1 (banana tag)
            prj = new ProjectInfo()
            {
                ProjectName = "Pineapple Tool",
                RepositoryId = snapshotRepo1.Id,
                RepositoryStamp = "1.0",
                Components =
                {
                    new ComponentManifest()
                    {
                        Tags = new List<string>() {"Banana"}
                    }
                }
            };
            await database.Upsert(prj).ConfigureAwait(false);
            //add a project that does not match query but sits in latest version
            prj = new ProjectInfo()
            {
                ProjectName = "Apple Tool",
                RepositoryId = snapshotRepo1.Id,
                RepositoryStamp = "2.0",
                Components =
                {
                    new ComponentManifest()
                    {
                        Tags = new List<string>() {"Apple"}
                    }
                }
            };
            await database.Upsert(prj).ConfigureAwait(false);
            return new SeedResult()
            {
                Repository = snapshotRepo1,
                MatchingProjects = new List<ProjectInfo>() {bananaToolV2}
            };
        }

        void AssertOneProjectFoundBy(string @by, ManifestQueryResult result1, ProjectInfo projectInfo)
        {
            result1.Projects.Count.Should().Be(1, $"searching by {by} part should work and return a single project");
            result1.Projects.Single().ProjectInfo.Id.Should().Be(projectInfo.Id,
                $"searching by {by} part should return the right project");
        }

        async Task<ManifestQueryResult> GetResultFromRepoOne(RepositoryManagementService repositoryManagementService, string query)
        {
            ManifestQueryResult manifestQueryResult = await repositoryManagementService.GetCurrentProjects(
                new RepositoryQueryParameter(this.testRepoOne.OrganizationName.ToUpperInvariant(),
                    this.testRepoOne.RepositoryName.ToUpperInvariant())
                , query, false).ConfigureAwait(false);
            return manifestQueryResult;
        }

        private async Task AddMoreProjectsToEnsureNotTooMuchIsReturned(RepoCat.Transmission.Models.ProjectInfo prj, RepositoryManagementService service,
         ProjectInfo insertedProject)
        {
            //add another project to the repo - this is in order to ensure that the 'get' method actually filters and not returns everything
            //which in case of a repo with one project would be a false test
            var anotherPrj = new RepoCat.Transmission.Models.ProjectInfo()
            {

                AssemblyName = "SomeAssemblyName",
                RepositoryInfo = new RepoCat.Transmission.Models.RepositoryInfo()
                {
                    RepositoryName = prj.RepositoryInfo.RepositoryName,
                    OrganizationName = prj.RepositoryInfo.OrganizationName
                },
                ProjectName = "SomeProjectName",
                DownloadLocation = "SomeDownloadLocation",
                RepositoryStamp = "23.23.12.5",
                ProjectUri = "SomeProjectUri",
                Autogenerated = false,
                DocumentationUri = "SomeDocsUri",
                OutputType = "Library",
                Owner = "SomeGuy",
                ProjectDescription = "Project That Does Something",
                TargetExtension = ".dll",
                Tags = { "Some", "Cool", "Features" },
                Properties = { { "ProjectProp", "PropVal" } },
                Components =
                {
                    new RepoCat.Transmission.Models.ComponentManifest(
                        new List<string>() {"Some", "Project", "Tags"},
                        new Dictionary<string, string>()
                        {
                            {"ComponentType", "SomeType"},
                            {"Author", "SomeGuy"}
                        })
                    {
                        Name = "SomeComponent",
                        Description = "Some component does something",
                        DocumentationUri = "SomeFakeComponentURI",

                    },
                },
            };

            await service.Upsert(anotherPrj).ConfigureAwait(false);

            //add same project to different repo
            prj.RepositoryInfo.RepositoryName = this.testRepoTwo.RepositoryName;
            prj.RepositoryInfo.OrganizationName = this.testRepoTwo.OrganizationName;
            var insertedProject2 = await service.Upsert(prj).ConfigureAwait(false);
            insertedProject.Id.Should().NotBe(insertedProject2.Id);

        }

        private static RepoCat.Transmission.Models.ProjectInfo GetEmptyProject(RepositoryInfo repositoryInfo, string name = null)
        {
            return new RepoCat.Transmission.Models.ProjectInfo()
            {
                ProjectName = name,
                RepositoryInfo = new RepoCat.Transmission.Models.RepositoryInfo()
                {
                    RepositoryName = repositoryInfo.RepositoryName,
                    OrganizationName = repositoryInfo.OrganizationName
                }
            };
        }


        private static async Task<SeedResult> SeedSnapshotRepoTwoForMultiRepoQuery(RepositoryDatabase database)
        {
            RepositoryInfo snapshotRepo2 = await database.UpsertUpdate(
                new RepositoryInfo()
                {
                    OrganizationName = $"Organization_{TestHelpers.GetMethodName()}",
                    RepositoryName = $"Repository_{TestHelpers.GetMethodName()}",
                    RepositoryMode = RepositoryMode.Snapshot
                }).ConfigureAwait(false);
            //add project 1V1
            ProjectInfo prj = new ProjectInfo()
            {
                ProjectName = "Banana Tool",
                RepositoryId = snapshotRepo2.Id,
                RepositoryStamp = "5.0",
                Components =
                {
                    new ComponentManifest()
                    {
                        Tags = new List<string>() {"TagFromV1"}
                    }
                }
            };
            await database.Upsert(prj).ConfigureAwait(false);
            //add v2 of the same project
            prj = new ProjectInfo()
            {
                ProjectName = "Banana Tool",
                RepositoryId = snapshotRepo2.Id,
                RepositoryStamp = "6.0",
                Components =
                {
                    new ComponentManifest()
                    {
                        Tags = new List<string>() {"VersionTwoTag"}
                    }
                }
            };
            var bananaToolV2 = await database.Upsert(prj).ConfigureAwait(false);

            //add different project that could be found with the same query to v1 (banana tag)
            prj = new ProjectInfo()
            {
                ProjectName = "Pineapple Tool",
                RepositoryId = snapshotRepo2.Id,
                RepositoryStamp = "5.0",
                Components =
                {
                    new ComponentManifest()
                    {
                        Tags = new List<string>() {"Banana"}
                    }
                }
            };
            await database.Upsert(prj).ConfigureAwait(false);
            //add a project that does not match query but sits in latest version
            prj = new ProjectInfo()
            {
                ProjectName = "Apple Tool",
                RepositoryId = snapshotRepo2.Id,
                RepositoryStamp = "6.0",
                Components =
                {
                    new ComponentManifest()
                    {
                        Tags = new List<string>() {"Apple"}
                    }
                }
            };
            await database.Upsert(prj).ConfigureAwait(false);
            return new SeedResult()
            {
                Repository = snapshotRepo2,
                MatchingProjects = new List<ProjectInfo>() {bananaToolV2}
            };
        }
    }
}