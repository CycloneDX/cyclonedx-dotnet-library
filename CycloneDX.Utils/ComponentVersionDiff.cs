using System;
using System.Collections.Generic;
using System.Linq;
using CycloneDX.Models.v1_2;

namespace CycloneDX.Utils
{
    public class DiffItem<T>
    {
        public List<T> Added { get; set; } = new List<T>();
        public List<T> Removed { get; set; } = new List<T>();
        public List<T> Unchanged { get; set; } = new List<T>();
    }

    public static partial class CycloneDXUtils
    {
        private static string ComponentDiffIdentifier(Component component)
        {
            var componentIdentifier = $"{component.Group}:{component.Name}";
            if (componentIdentifier.StartsWith(":")) componentIdentifier = componentIdentifier.Substring(1);
            return componentIdentifier;
        }

        public static Dictionary<string, DiffItem<Component>> ComponentVersionDiff(Bom fromBom, Bom toBom)
        {
            var result = new Dictionary<string, DiffItem<Component>>();

            // make a copy of components that are still to be processed
            var fromComponents = new List<Component>(fromBom.Components);
            var toComponents = new List<Component>(toBom.Components);
            
            // unchanged component versions
            // loop over the toBom and fromBom Components list as we will be modifying the fromComponents list
            foreach (var fromComponent in fromBom.Components)
            {
                // if component version is in both SBOMs
                if (toBom.Components.Count(toComponent =>
                        toComponent.Group == fromComponent.Group
                        && toComponent.Name == fromComponent.Name
                        && toComponent.Version == fromComponent.Version
                    ) > 0)
                {
                    var componentIdentifier = ComponentDiffIdentifier(fromComponent);

                    if (!result.ContainsKey(componentIdentifier))
                    {
                        result.Add(componentIdentifier, new DiffItem<Component>());
                    }

                    result[componentIdentifier].Unchanged.Add(fromComponent);

                    fromComponents.RemoveAll(c => c.Group == fromComponent.Group && c.Name == fromComponent.Name && c.Version == fromComponent.Version);
                    toComponents.RemoveAll(c => c.Group == fromComponent.Group && c.Name == fromComponent.Name && c.Version == fromComponent.Version);
                }
            }

            // added component versions
            foreach (var component in new List<Component>(toComponents))
            {
                var componentIdentifier = ComponentDiffIdentifier(component);
                if (!result.ContainsKey(componentIdentifier))
                {
                    result.Add(componentIdentifier, new DiffItem<Component>());
                }

                result[componentIdentifier].Added.Add(component);
            }

            // removed components versions
            foreach (var component in new List<Component>(fromComponents))
            {
                var componentIdentifier = ComponentDiffIdentifier(component);
                if (!result.ContainsKey(componentIdentifier))
                {
                    result.Add(componentIdentifier, new DiffItem<Component>());
                }

                result[componentIdentifier].Removed.Add(component);
            }

            return result;
        }
    }
}
