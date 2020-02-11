using AutoMapper;
using NUnit.Framework;
using RepoCat.Portal;
using RepoCat.Portal.Mapping;
using RepoCat.Portal.Utilities;
using RepoCat.Transmission.Models;

namespace RepoCat.Tests
{
    [TestFixture]
    public class MapperTests
    {

        [Test]
        public void AssertConfigIsValid()
        {
            var mappingConfig = MappingConfigurationFactory.Create();
            mappingConfig.AssertConfigurationIsValid();

        }
    }
}