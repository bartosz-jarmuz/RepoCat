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
    public class SearchStatisticsTests
    {
        private static readonly IRepoCatDbSettings Settings = TestHelpers.GetSettings();
        

        [SetUp]
        public void SetUp()
        {
            MongoClient client = new MongoClient(Settings.ConnectionString);
            client.DropDatabase(Settings.DatabaseName);
        }

        [TearDown]
        public void TearDown()
        {
            
        }

        [Test]
        public async Task AddSameTags_ValuesIncrementedProperly()
        {
            var db = new StatisticsDatabase(Settings);
            StatisticsService service = new StatisticsService(db, new Mapper(MappingConfigurationFactory.Create()));
            string organizationOne = Guid.NewGuid().ToString();
            string repoOne = Guid.NewGuid().ToString();

            var parameter = new RepositoryQueryParameter(organizationOne, repoOne);
            string[] words = new[] {"FirstTag", "SecondTag", "Third"};
            SearchStatistics stats1 = await service.Update(parameter, words).ConfigureAwait(false);
            SearchStatistics stats2 = await service.Update(parameter, words).ConfigureAwait(false);

            stats1.RepositoryName.Should().Be(stats2.RepositoryName);
            stats1.SearchKeywordData.Count.Should().Be(3);
            foreach (SearchKeywordData searchKeywordData in stats1.SearchKeywordData)
            {
                Assert.AreEqual(1, searchKeywordData.SearchCount);
            }
            stats2.SearchKeywordData.Count.Should().Be(3);
            foreach (SearchKeywordData searchKeywordData in stats2.SearchKeywordData)
            {
                Assert.AreEqual(2, searchKeywordData.SearchCount);
            }
        }
    }
}