using System;
using System.Collections.Generic;
using System.Linq;
using CycloneDX.Models.v1_3;

namespace CycloneDX.Utils
{
    public static partial class CycloneDXUtils
    {
        public static Dictionary<string, List<Component>> MultipleComponentVersions(Bom bom)
        {
            var result = new Dictionary<string, List<Component>>();

            var componentCache = new Dictionary<string, List<Component>>();

            foreach (var component in bom.Components)
            {
                var componentIdentifier = ComponentAnalysisIdentifier(component);
                if (!componentCache.ContainsKey(componentIdentifier))
                {
                    componentCache[componentIdentifier] = new List<Component>();
                }
                componentCache[componentIdentifier].Add(component);
            }

            foreach (var componentEntry in componentCache)
            {
                if (componentEntry.Value.Count > 1)
                {
                    var firstVersion = componentEntry.Value.First().Version;
                    foreach (var component in componentEntry.Value)
                    {
                        if (component.Version != firstVersion)
                        {
                            result[componentEntry.Key] = componentEntry.Value;
                            break;
                        }
                    }
                }
            }

            return result;
        }
    }
}
