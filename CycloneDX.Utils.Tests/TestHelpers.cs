using System;
using System.Collections.Generic;
using CycloneDX.Models.v1_2;

namespace CycloneDX.Utils.Tests
{
    public static class Helpers
    {
        public static Bom ComponentBomHelper(IEnumerable<string> componentNameAndVersions)
        {
            var bom = new Bom
            {
                Components = new List<Component>()
            };

            foreach (var componentNameAndVersion in componentNameAndVersions)
            {
                bom.Components.Add(new Component
                {
                    Name = componentNameAndVersion.Split('@')[0],
                    Version = componentNameAndVersion.Split('@')[1]
                });
            }

            return bom;
        }
    }
}