// -----------------------------------------------------------------------
//  <copyright file="CleanupTests.cs" company="bartosz.jarmuz@gmail.com">
//   Copyright (c) Bartosz Jarmuż. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using MongoDB.Bson;
using MongoDB.Driver;
using NFluent;
using NUnit.Framework;
using RepoCat.Persistence.Models;
using RepoCat.Persistence.Service;
using RepoCat.Portal.Mapping;
using RepoCat.RepositoryManagement.Service;

namespace Repocat.Persistence.Tests
{

    [TestFixture]
    public class CleanupTests
    {
        private static readonly IRepoCatDbSettings Settings = TestHelpers.GetSettings();

        private readonly RepositoryInfo snapshotRepoOne = new RepositoryInfo()
        {
            Id = ObjectId.GenerateNewId(),
            RepositoryName = "snapshotRepoOne",
            OrganizationName = "TestOrg",
            RepositoryMode = RepositoryMode.Snapshot
            
        };

        private readonly RepositoryInfo snapshotRepoTwo = new RepositoryInfo()
        {
            Id = ObjectId.GenerateNewId(),
            RepositoryName = "snapshotRepoTwo",
            OrganizationName = "TestOrgTwo",
            RepositoryMode = RepositoryMode.Snapshot
        };

        private readonly RepositoryInfo defaultRepoOne = new RepositoryInfo()
        {
            Id = ObjectId.GenerateNewId(),
            RepositoryName = "defaultRepoOne",
            OrganizationName = "TestOrg",
        };

        [SetUp]
        public void SetUp()
        {
            MongoClient client = new MongoClient(Settings.ConnectionString);
            client.DropDatabase(Settings.DatabaseName);

            RepositoryDatabase database = new RepositoryDatabase(Settings);
            RepositoryInfo result = database.Create(this.snapshotRepoOne);
            Assert.IsNotNull(result);
            RepositoryInfo result2 = database.Create(this.snapshotRepoTwo);
            Assert.IsNotNull(result2);
            RepositoryInfo result3 = database.Create(this.defaultRepoOne);
            Assert.IsNotNull(result3);
        }

        [TearDown]
        public void TearDown()
        {
        }


        [Test]
        public async Task DefaultRepoProjects_NotCleanedUp()
        {
            //arrange
            RepositoryDatabase database = new RepositoryDatabase(Settings);
            var service = new RepositoryManagementService(database, new Mapper(MappingConfigurationFactory.Create()), TelemetryMock.InitializeMockTelemetryClient());
            await SeedProjects(service, "Project1", this.defaultRepoOne, "1.0.0", "2.0.0", "3.0.0", "4.0.0");
            await SeedProjects(service, "Project2", this.defaultRepoOne, "1.0.0", "2.0.0", "3.0.0", "4.0.0");
            await SeedProjects(service, "Project3", this.defaultRepoOne, "1.0.0", "2.0.0", "3.0.0", "4.0.0");
            await SeedProjects(service, "Project4", this.defaultRepoOne, "1.0.0", "2.0.0", "3.0.0", "4.0.0");
            SnapshotRepoCleaner cleaner = new SnapshotRepoCleaner(database);
            
            //assuming...
            Check.That(database.GetAllProjects().Result.Count()).Equals(4);

            //act
            SnapshotRepoCleanupResult result = await cleaner.PerformCleanupAsync(new SnapshotRepoCleanupSettings() { });

            //assert
            Check.That(result.RepositoryResults.Count).IsEqualTo(0);
            Check.That(database.GetAllProjects().Result.Count()).Equals(4);
        }

        [Test]
        public async Task SnapshotRepoProjects_SettingsChange_AffectsTheNumberOfProjectsLeft()
        {
            //arrange
            RepositoryDatabase database = new RepositoryDatabase(Settings);
            var service = new RepositoryManagementService(database, new Mapper(MappingConfigurationFactory.Create()), TelemetryMock.InitializeMockTelemetryClient());
            await SeedProjects(service, "Project1", this.snapshotRepoOne, "1.0.0", "2.0.0", "3.0.0", "4.0.0");
            SnapshotRepoCleaner cleaner = new SnapshotRepoCleaner(database);

            //assuming...
            Check.That(database.GetAllProjects().Result.Count()).Equals(4);

            //act
            SnapshotRepoCleanupResult result = await cleaner.PerformCleanupAsync(new SnapshotRepoCleanupSettings() { NumberOfSnapshotsToKeep = 3});

            //assert
            Check.That(result.RepositoryResults.Single().Value).IsEqualTo(1);
            Check.That(database.GetAllProjects().Result.Count()).Equals(3);

            //now change settings and run again

            //act
            result = await cleaner.PerformCleanupAsync(new SnapshotRepoCleanupSettings() { NumberOfSnapshotsToKeep = 1 });

            //assert
            Check.That(result.RepositoryResults.Single().Value).IsEqualTo(2);
            Check.That(database.GetAllProjects().Result.Count()).Equals(1);
        }

        private static async Task SeedProjects(RepositoryManagementService service, string projectName, RepositoryInfo  repo, params string[] stamps)
        {
            foreach (string stamp in stamps)
            {
                RepoCat.Transmission.Models.ProjectInfo prj = new RepoCat.Transmission.Models.ProjectInfo()
                {
                    ProjectName = projectName,
                    RepositoryStamp = stamp,
                    RepositoryInfo  =new RepoCat.Transmission.Models.RepositoryInfo()
                    {
                        OrganizationName = repo.OrganizationName,
                        RepositoryName = repo.RepositoryName
                    }
                };
                await service.Upsert(prj).ConfigureAwait(false);
            }
        }

        [Test]
        public async Task TestMultipleRepos_RelevantProjectsDeleted()
        {
            //arrange
            RepositoryDatabase database = new RepositoryDatabase(Settings);
            var service = new RepositoryManagementService(database, new Mapper(MappingConfigurationFactory.Create()), TelemetryMock.InitializeMockTelemetryClient());
            await SeedProjects(service, "Project1", this.snapshotRepoOne, "1.0.0", "2.0.0", "3.0.0");
            await SeedProjects(service, "Project2", this.snapshotRepoOne, "1.0.0");
            await SeedProjects(service, "Project3", this.snapshotRepoTwo, "1.0.0", "2.0.0", "3.0.0", "4.0.0");
            await SeedProjects(service, "Project4", this.snapshotRepoTwo, "1.0.0", "4.0.0");
            await SeedProjects(service, "Project5", this.defaultRepoOne, "1.0.0", "5.0.0");
            SnapshotRepoCleaner cleaner = new SnapshotRepoCleaner(database);

            //assuming...
            Check.That(database.GetAllProjects().Result.Count()).Equals(11);

            //act
            SnapshotRepoCleanupResult result = await cleaner.PerformCleanupAsync(new SnapshotRepoCleanupSettings());

            //assert
            Check.That(result.RepositoryResults.Keys.Select(x=>x.RepositoryName)).ContainsExactly(this.snapshotRepoOne.RepositoryName, this.snapshotRepoTwo.RepositoryName);
            Check.That(result.RepositoryResults.Single(x=>x.Key.Id == this.snapshotRepoOne.Id).Value.Equals(2));
            Check.That(result.RepositoryResults.Single(x=>x.Key.Id == this.snapshotRepoTwo.Id).Value.Equals(3));
            
            var allProjects = database.GetAllProjects().Result.ToList();
            Check.That(allProjects.Count()).Equals(6);

            Check.That(allProjects.Where(x => x.RepositoryId == this.snapshotRepoOne.Id).Select(x => x.ProjectName))
                .ContainsExactly("Project1", "Project1");

            Check.That(allProjects.Where(x => x.RepositoryId == this.snapshotRepoTwo.Id).Select(x => x.ProjectName))
                .ContainsExactly("Project3", "Project3", "Project4");

            Check.That(allProjects.Where(x => x.RepositoryId == this.defaultRepoOne.Id).Select(x => x.ProjectName))
                .ContainsExactly("Project5");

        }

    }
}