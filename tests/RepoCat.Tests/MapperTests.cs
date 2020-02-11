// -----------------------------------------------------------------------
//  <copyright file="MapperTests.cs" company="bartosz.jarmuz@gmail.com">
//   Copyright (c) Bartosz Jarmuż. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using NUnit.Framework;
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