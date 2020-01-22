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
using RepoCat.Persistence.Service;
using RepoCat.Portal.Mapping;
using RepoCat.RepositoryManagement.Service;
using RepoCat.Transmission.Models;
using ProjectInfo = RepoCat.Persistence.Models.ProjectInfo;
using RepositoryInfo = RepoCat.Persistence.Models.RepositoryInfo;
using RepositoryMode = RepoCat.Persistence.Models.RepositoryMode;
using RepositoryQueryParameter = RepoCat.RepositoryManagement.Service.RepositoryQueryParameter;
using SearchKeywordData = RepoCat.RepositoryManagement.Service.SearchKeywordData;

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



        [Test]
        public async Task AddSameTags_VariousRepos_FlattenedProperly()
        {
            //arrange
            var db = new StatisticsDatabase(Settings);
            StatisticsService service = new StatisticsService(db, new Mapper(MappingConfigurationFactory.Create()));

            var parameter1 = new RepositoryQueryParameter(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
            await service.Update(parameter1, new []{ "FirstTag", "SecondTag", "Third" }).ConfigureAwait(false);
            await service.Update(parameter1, new[] { "FIRSTTAG", "", "Fourth" }).ConfigureAwait(false);
            await service.Update(parameter1, new[] { "FIRSTTAG", "SecondTag", "Fourth" }).ConfigureAwait(false);


            parameter1 = new RepositoryQueryParameter(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
            await service.Update(parameter1, new[] { "FirstTag", "SecondTag", "Fourth" }).ConfigureAwait(false);
            await service.Update(parameter1, new[] { "Fifth", "Sixth"}).ConfigureAwait(false);

            //act
            List<SearchKeywordData> result = (await service.GetFlattened().ConfigureAwait(false)).ToList();


            //assert
            result.Count.Should().Be(6);
            result.Single(x => x.Keyword == "FirstTag").SearchCount.Should().Be(4);
            result.Single(x => x.Keyword == "SecondTag").SearchCount.Should().Be(3);
            result.Single(x => x.Keyword == "Third").SearchCount.Should().Be(1);
            result.Single(x => x.Keyword == "Fourth").SearchCount.Should().Be(3);
            result.Single(x => x.Keyword == "Fifth").SearchCount.Should().Be(1);
            result.Single(x => x.Keyword == "Sixth").SearchCount.Should().Be(1);

        }
    }
}