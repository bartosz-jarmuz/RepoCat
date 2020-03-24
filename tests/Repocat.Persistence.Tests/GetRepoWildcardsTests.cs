// -----------------------------------------------------------------------
//  <copyright file="GetProjectTests.cs" company="bartosz.jarmuz@gmail.com">
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
using NUnit.Framework;
using RepoCat.Persistence.Models;
using RepoCat.Persistence.Service;
using RepoCat.Portal.Mapping;
using RepoCat.RepositoryManagement.Service;
using PropertiesCollection = RepoCat.Transmission.Models.PropertiesCollection;
using RepositoryQueryParameter = RepoCat.RepositoryManagement.Service.RepositoryQueryParameter;

namespace Repocat.Persistence.Tests
{
    [TestFixture]
    public class GetRepoWildcardsTests
    {
        private static readonly IRepoCatDbSettings Settings = TestHelpers.GetSettings();

        // ReSharper disable InconsistentNaming
        private readonly RepositoryInfo orgOne_RepoOne = new RepositoryInfo()
            {
            Id = ObjectId.GenerateNewId(),
            RepositoryName = "TestRepoOne",
            OrganizationName = "OrgOne"
        };

        private readonly RepositoryInfo orgOne_RepoTwo = new RepositoryInfo()
        {
            Id = ObjectId.GenerateNewId(),
            RepositoryName = "TestRepoTwo",
            OrganizationName = "OrgOne"
        };

        private readonly RepositoryInfo orgTwo_RepoOne = new RepositoryInfo()
        {
            Id = ObjectId.GenerateNewId(),
            RepositoryName = "TestRepoOne",
            OrganizationName = "OrgTwo"
        };

        private readonly RepositoryInfo orgTwo_RepoTwo = new RepositoryInfo()
        {
            Id = ObjectId.GenerateNewId(),
            RepositoryName = "TestRepoTwo",
            OrganizationName = "OrgTwo"
        }; 
        
        private readonly RepositoryInfo orgThree_RepoOne = new RepositoryInfo()
        {
            Id = ObjectId.GenerateNewId(),
            RepositoryName = "TestRepoOne",
            OrganizationName = "OrgThree"
        };

        private readonly RepositoryInfo orgThree_RepoTwo = new RepositoryInfo()
        {
            Id = ObjectId.GenerateNewId(),
            RepositoryName = "TestRepoTwo",
            OrganizationName = "OrgThree"
        };

        private readonly RepositoryInfo orgThree_RepoThree = new RepositoryInfo()
        {
            Id = ObjectId.GenerateNewId(),
            RepositoryName = "TestRepoThree",
            OrganizationName = "OrgThree"
        };


        // ReSharper restore InconsistentNaming

        [SetUp]
        public void SetUp()
        {
            MongoClient client = new MongoClient(Settings.ConnectionString);
            client.DropDatabase(Settings.DatabaseName);

            RepositoryDatabase database = new RepositoryDatabase(Settings);
            RepositoryInfo result = database.Create(this.orgOne_RepoOne);
            Assert.IsNotNull(result);
            this.SeedProjects(database, result).GetAwaiter().GetResult();
            
            result = database.Create(this.orgOne_RepoTwo);
            Assert.IsNotNull(result);
            this.SeedProjects(database, result).GetAwaiter().GetResult();

            result = database.Create(this.orgTwo_RepoOne);
            Assert.IsNotNull(result);
            this.SeedProjects(database, result).GetAwaiter().GetResult();

            result = database.Create(this.orgTwo_RepoTwo);
            Assert.IsNotNull(result);
            this.SeedProjects(database, result).GetAwaiter().GetResult();
            
            result = database.Create(this.orgThree_RepoOne);
            Assert.IsNotNull(result);
            this.SeedProjects(database, result).GetAwaiter().GetResult();

            result = database.Create(this.orgThree_RepoTwo);
            Assert.IsNotNull(result);
            this.SeedProjects(database, result).GetAwaiter().GetResult();

            result = database.Create(this.orgThree_RepoThree);
            Assert.IsNotNull(result);
            this.SeedProjects(database, result).GetAwaiter().GetResult();

        }

        private async Task SeedProjects(RepositoryDatabase database, RepositoryInfo repo)
        {
            ProjectInfo prj = new ProjectInfo()
            {
                ProjectName = $"{repo.OrganizationName}_{repo.RepositoryName}_Prj",
                RepositoryId = repo.Id,
                Components =
                {
                    new ComponentManifest()
                    {
                        Tags = new List<string>() {"FindMe"}
                    }
                }
            };
            await database.Upsert(prj).ConfigureAwait(false);
        }


        [TearDown]
        public void TearDown()
        {
        }



        [Test]
        public async Task TestGet_GlobalWildcard_ReturnsAll()
        {
            //arrange
            RepositoryDatabase database = new RepositoryDatabase(Settings);
            var service = new RepositoryManagementService(database, new Mapper(MappingConfigurationFactory.Create()),
                TelemetryMock.InitializeMockTelemetryClient());
            var parameters = new[] {new RepositoryQueryParameter("*", "*")};

            //act 
            ManifestQueryResult result = await service.GetCurrentProjects(parameters, "FindMe",false);

            //assert
            Assert.AreEqual(7, result.Projects.Count);
            Assert.AreEqual(1, result.Projects.Count(x=>x.RepositoryInfo.Id == this.orgOne_RepoOne.Id));
            Assert.AreEqual(1, result.Projects.Count(x=>x.RepositoryInfo.Id == this.orgOne_RepoTwo.Id));
            Assert.AreEqual(1, result.Projects.Count(x=>x.RepositoryInfo.Id == this.orgTwo_RepoOne.Id));
            Assert.AreEqual(1, result.Projects.Count(x=>x.RepositoryInfo.Id == this.orgTwo_RepoTwo.Id));
            Assert.AreEqual(1, result.Projects.Count(x=>x.RepositoryInfo.Id == this.orgThree_RepoOne.Id));
            Assert.AreEqual(1, result.Projects.Count(x=>x.RepositoryInfo.Id == this.orgThree_RepoTwo.Id));
            Assert.AreEqual(1, result.Projects.Count(x=>x.RepositoryInfo.Id == this.orgThree_RepoThree.Id));
        }

        [Test]
        public async Task TestGet_GlobalWildcardAndSpecificRepo_ReturnsNoDuplicates()
        {
            //arrange
            RepositoryDatabase database = new RepositoryDatabase(Settings);
            var service = new RepositoryManagementService(database, new Mapper(MappingConfigurationFactory.Create()),
                TelemetryMock.InitializeMockTelemetryClient());
            var parameters = new[] { new RepositoryQueryParameter("*", "*"), new RepositoryQueryParameter(this.orgThree_RepoThree),  };

            //act 
            ManifestQueryResult result = await service.GetCurrentProjects(parameters, "FindMe", false);

            //assert
            Assert.AreEqual(7, result.Projects.Count);
            Assert.AreEqual(1, result.Projects.Count(x => x.RepositoryInfo.Id == this.orgOne_RepoOne.Id));
            Assert.AreEqual(1, result.Projects.Count(x => x.RepositoryInfo.Id == this.orgOne_RepoTwo.Id));
            Assert.AreEqual(1, result.Projects.Count(x => x.RepositoryInfo.Id == this.orgTwo_RepoOne.Id));
            Assert.AreEqual(1, result.Projects.Count(x => x.RepositoryInfo.Id == this.orgTwo_RepoTwo.Id));
            Assert.AreEqual(1, result.Projects.Count(x => x.RepositoryInfo.Id == this.orgThree_RepoOne.Id));
            Assert.AreEqual(1, result.Projects.Count(x => x.RepositoryInfo.Id == this.orgThree_RepoTwo.Id));
            Assert.AreEqual(1, result.Projects.Count(x => x.RepositoryInfo.Id == this.orgThree_RepoThree.Id));
        }

        [Test]
        public async Task TestGet_RepoWildcard_ReturnsOnlySpecifiedOrg()
        {
            //arrange
            RepositoryDatabase database = new RepositoryDatabase(Settings);
            var service = new RepositoryManagementService(database, new Mapper(MappingConfigurationFactory.Create()),
                TelemetryMock.InitializeMockTelemetryClient());
            var parameters = new[] { new RepositoryQueryParameter(this.orgOne_RepoOne.OrganizationName, "*") };

            //act 
            ManifestQueryResult result = await service.GetCurrentProjects(parameters, "FindMe", false);

            //assert
            Assert.AreEqual(2, result.Projects.Count);
            Assert.AreEqual(1, result.Projects.Count(x => x.RepositoryInfo.Id == this.orgOne_RepoOne.Id));
            Assert.AreEqual(1, result.Projects.Count(x => x.RepositoryInfo.Id == this.orgOne_RepoTwo.Id));
            Assert.AreEqual(0, result.Projects.Count(x => x.RepositoryInfo.Id == this.orgTwo_RepoOne.Id));
            Assert.AreEqual(0, result.Projects.Count(x => x.RepositoryInfo.Id == this.orgTwo_RepoTwo.Id));
            Assert.AreEqual(0, result.Projects.Count(x => x.RepositoryInfo.Id == this.orgThree_RepoOne.Id));
            Assert.AreEqual(0, result.Projects.Count(x => x.RepositoryInfo.Id == this.orgThree_RepoTwo.Id));
            Assert.AreEqual(0, result.Projects.Count(x => x.RepositoryInfo.Id == this.orgThree_RepoThree.Id));
        }

        [Test]
        public async Task TestGet_RepoWildcardAndSpecificRepo_ReturnsOnlySpecifiedOrgs()
        {
            //arrange
            RepositoryDatabase database = new RepositoryDatabase(Settings);
            var service = new RepositoryManagementService(database, new Mapper(MappingConfigurationFactory.Create()),
                TelemetryMock.InitializeMockTelemetryClient());
            var parameters = new[] { new RepositoryQueryParameter(this.orgOne_RepoOne.OrganizationName, "*"), new RepositoryQueryParameter(this.orgTwo_RepoOne),  };

            //act 
            ManifestQueryResult result = await service.GetCurrentProjects(parameters, "FindMe", false);

            //assert
            Assert.AreEqual(3, result.Projects.Count);
            Assert.AreEqual(1, result.Projects.Count(x => x.RepositoryInfo.Id == this.orgOne_RepoOne.Id));
            Assert.AreEqual(1, result.Projects.Count(x => x.RepositoryInfo.Id == this.orgOne_RepoTwo.Id));
            Assert.AreEqual(1, result.Projects.Count(x => x.RepositoryInfo.Id == this.orgTwo_RepoOne.Id));
            Assert.AreEqual(0, result.Projects.Count(x => x.RepositoryInfo.Id == this.orgTwo_RepoTwo.Id));
            Assert.AreEqual(0, result.Projects.Count(x => x.RepositoryInfo.Id == this.orgThree_RepoOne.Id));
            Assert.AreEqual(0, result.Projects.Count(x => x.RepositoryInfo.Id == this.orgThree_RepoTwo.Id));
            Assert.AreEqual(0, result.Projects.Count(x => x.RepositoryInfo.Id == this.orgThree_RepoThree.Id));
        }


        [Test]
        public async Task TestGet_RepoWildcardLast_ReturnsOnlySpecifiedOrgs()
        {
            //arrange
            RepositoryDatabase database = new RepositoryDatabase(Settings);
            var service = new RepositoryManagementService(database, new Mapper(MappingConfigurationFactory.Create()),
                TelemetryMock.InitializeMockTelemetryClient());
            var parameters = new[]
            {
                new RepositoryQueryParameter(this.orgOne_RepoOne), 
                new RepositoryQueryParameter(this.orgTwo_RepoOne),
                new RepositoryQueryParameter(this.orgThree_RepoOne),
                new RepositoryQueryParameter(this.orgThree_RepoOne.OrganizationName, "*"),
            };

            //act 
            ManifestQueryResult result = await service.GetCurrentProjects(parameters, "FindMe", false);

            //assert
            Assert.AreEqual(5, result.Projects.Count);
            Assert.AreEqual(1, result.Projects.Count(x => x.RepositoryInfo.Id == this.orgOne_RepoOne.Id));
            Assert.AreEqual(0, result.Projects.Count(x => x.RepositoryInfo.Id == this.orgOne_RepoTwo.Id));
            Assert.AreEqual(1, result.Projects.Count(x => x.RepositoryInfo.Id == this.orgTwo_RepoOne.Id));
            Assert.AreEqual(0, result.Projects.Count(x => x.RepositoryInfo.Id == this.orgTwo_RepoTwo.Id));
            Assert.AreEqual(1, result.Projects.Count(x => x.RepositoryInfo.Id == this.orgThree_RepoOne.Id));
            Assert.AreEqual(1, result.Projects.Count(x => x.RepositoryInfo.Id == this.orgThree_RepoTwo.Id));
            Assert.AreEqual(1, result.Projects.Count(x => x.RepositoryInfo.Id == this.orgThree_RepoThree.Id));
        }

        [Test]
        public async Task TestGet_NoWildcards_ReturnsOnlySpecifiedOrgs()
        {
            //arrange
            RepositoryDatabase database = new RepositoryDatabase(Settings);
            var service = new RepositoryManagementService(database, new Mapper(MappingConfigurationFactory.Create()),
                TelemetryMock.InitializeMockTelemetryClient());
            var parameters = new[]
            {
                new RepositoryQueryParameter(this.orgOne_RepoOne),
                new RepositoryQueryParameter(this.orgTwo_RepoOne),
                new RepositoryQueryParameter(this.orgThree_RepoOne),
            };

            //act 
            ManifestQueryResult result = await service.GetCurrentProjects(parameters, "FindMe", false);

            //assert
            Assert.AreEqual(3, result.Projects.Count);
            Assert.AreEqual(1, result.Projects.Count(x => x.RepositoryInfo.Id == this.orgOne_RepoOne.Id));
            Assert.AreEqual(0, result.Projects.Count(x => x.RepositoryInfo.Id == this.orgOne_RepoTwo.Id));
            Assert.AreEqual(1, result.Projects.Count(x => x.RepositoryInfo.Id == this.orgTwo_RepoOne.Id));
            Assert.AreEqual(0, result.Projects.Count(x => x.RepositoryInfo.Id == this.orgTwo_RepoTwo.Id));
            Assert.AreEqual(1, result.Projects.Count(x => x.RepositoryInfo.Id == this.orgThree_RepoOne.Id));
            Assert.AreEqual(0, result.Projects.Count(x => x.RepositoryInfo.Id == this.orgThree_RepoTwo.Id));
            Assert.AreEqual(0, result.Projects.Count(x => x.RepositoryInfo.Id == this.orgThree_RepoThree.Id));
        }
    }
}