using System;
using System.Collections.Generic;
using Xunit;
using CycloneDX;
using CycloneDX.Models.v1_3;
using CycloneDX.Utils;

namespace CycloneDX.Utils.Tests
{
    public class ComponentVersionDiffTests
    {
        [Fact]
        public void ComponentRemovedTest()
        {
            var fromBom = Helpers.ComponentBomHelper(new List<string> { "component@1" });
            var toBom = Helpers.ComponentBomHelper(new List<string>());

            var result = CycloneDXUtils.ComponentVersionDiff(fromBom, toBom);

            Assert.Single(result["component"].Removed);
        }

        [Fact]
        public void ComponentAddedTest()
        {
            var fromBom = Helpers.ComponentBomHelper(new List<string>());
            var toBom = Helpers.ComponentBomHelper(new List<string> { "component@1" });

            var result = CycloneDXUtils.ComponentVersionDiff(fromBom, toBom);

            Assert.Single(result["component"].Added);
        }

        [Fact]
        public void ComponentUnchangedTest()
        {
            var fromBom = Helpers.ComponentBomHelper(new List<string> { "component@1" });
            var toBom = Helpers.ComponentBomHelper(new List<string> { "component@1" });

            var result = CycloneDXUtils.ComponentVersionDiff(fromBom, toBom);

            Assert.Single(result["component"].Unchanged);
        }

        [Fact]
        public void ComponentModifiedTest()
        {
            var fromBom = Helpers.ComponentBomHelper(new List<string> { "component@1" });
            var toBom = Helpers.ComponentBomHelper(new List<string> { "component@2" });

            var result = CycloneDXUtils.ComponentVersionDiff(fromBom, toBom);

            Assert.Single(result["component"].Added);
            Assert.Single(result["component"].Removed);
        }
    }
}
