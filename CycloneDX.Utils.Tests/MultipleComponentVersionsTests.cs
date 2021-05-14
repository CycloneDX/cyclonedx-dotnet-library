using System;
using System.Collections.Generic;
using Xunit;
using CycloneDX;
using CycloneDX.Models.v1_3;
using CycloneDX.Utils;

namespace CycloneDX.Utils.Tests
{
    public class MultipleComponentVersionsTests
    {
        [Fact]
        public void MultipleComponentVersionTest()
        {
            var bom = Helpers.ComponentBomHelper(new List<string> { "component@1", "component@2" });

            var result = CycloneDXUtils.MultipleComponentVersions(bom);

            Assert.Equal(2, result["component"].Count);
        }
    }
}
