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
using RepoCat.Transmission.Models;
using ProjectInfo = RepoCat.Persistence.Models.ProjectInfo;
using RepositoryInfo = RepoCat.Persistence.Models.RepositoryInfo;
using RepositoryMode = RepoCat.Persistence.Models.RepositoryMode;
using RepositoryQueryParameter = RepoCat.RepositoryManagement.Service.RepositoryQueryParameter;

namespace Repocat.Persistence.Tests
{

    [TestFixture]
    public class UpsertRepoTests
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
        public async Task CannotAddSameTwice_ShouldReturnTheSameId()
        {
            RepositoryDatabase database = new RepositoryDatabase(Settings);
            string organizationOne = Guid.NewGuid().ToString();
            string repoOne = Guid.NewGuid().ToString();
            RepositoryInfo repo = null;
            RepositoryInfo repo2 = null;

            Task t1 = Task.Run(async () =>
            {
                repo = await database.UpsertUpdate(new RepositoryInfo()
                    {
                        OrganizationName = organizationOne,
                        RepositoryName = repoOne
                    }
                ).ConfigureAwait(false);
                
            });
            Task t2 = Task.Run(async () =>
            {
                repo2 = await database.UpsertUpdate(new RepositoryInfo()
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
        public async Task SnapshotModeSetFromStart()
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
        public async Task ShouldReturnProperMode()
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

        
    }
}