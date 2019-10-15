using NUnit.Framework;
using RepoCat.Portal;
using RepoCat.Portal.Mapping;

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