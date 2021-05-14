using System;
using System.Collections.Generic;
using System.Linq;
using CycloneDX.Models.v1_3;

namespace CycloneDX.Utils
{
    public static partial class CycloneDXUtils
    {
        public static string ComponentAnalysisIdentifier(Component component)
        {
            var componentIdentifier = $"{component.Group}:{component.Name}";
            if (componentIdentifier.StartsWith(":")) componentIdentifier = componentIdentifier.Substring(1);
            return componentIdentifier;
        }
    }
}
