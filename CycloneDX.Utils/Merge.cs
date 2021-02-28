using System;
using System.Collections.Generic;
using CycloneDX.Models.v1_2;

namespace CycloneDX.Utils
{
    public class MergeOptions
    {
        public bool Components { get; set; } = true;
    }

    public static partial class CycloneDXUtils
    {
        public static Bom Merge(IEnumerable<Bom> sboms)
        {
            return Merge(sboms, new MergeOptions());
        }

        public static Bom Merge(IEnumerable<Bom> sboms, MergeOptions options)
        {
            var result = new Bom();

            foreach (var sbom in sboms)
            {
                result = Merge(result, sbom, options);
            }

            return result;
        }

        public static Bom Merge(Bom sbom1, Bom sbom2)
        {
            return Merge(sbom1, sbom2, new MergeOptions());
        }

        public static Bom Merge(Bom sbom1, Bom sbom2, MergeOptions options)
        {
            var result = new Bom();

            if (options.Components)
            {
                if (sbom1.Components != null)
                {
                    result.Components = sbom1.Components;
                    if (sbom2.Components != null)
                    {
                        result.Components.AddRange(sbom2.Components);
                    }
                }
                else
                {
                    result.Components = sbom2.Components;
                }
            }

            return result;
        }
    }
}
