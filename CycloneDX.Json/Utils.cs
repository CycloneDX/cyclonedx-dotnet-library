using System.Text.Json;
using CycloneDX.Json;

namespace CycloneDX.Json
{
    internal static class Utils
    {
        public static void AddJsonConverters_v1_2(JsonSerializerOptions options)
        {
            options.Converters.Add(new v1_2.Converters.ComponentTypeConverter());
            options.Converters.Add(new v1_2.Converters.DataFlowConverter());
            options.Converters.Add(new v1_2.Converters.DateTimeConverter());
            options.Converters.Add(new v1_2.Converters.DependencyConverter());
            options.Converters.Add(new v1_2.Converters.ExternalReferenceTypeConverter());
            options.Converters.Add(new v1_2.Converters.HashAlgorithmConverter());
            options.Converters.Add(new v1_2.Converters.IssueClassificationConverter());
            options.Converters.Add(new v1_2.Converters.LicenseConverter());
            options.Converters.Add(new v1_2.Converters.PatchClassificationConverter());
        }
    }
}