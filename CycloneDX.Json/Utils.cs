using System.Text.Json;

namespace CycloneDX.Json
{
    internal static class Utils
    {
        public static void AddJsonConverters(JsonSerializerOptions options)
        {
            options.Converters.Add(new ComponentTypeConverter());
            options.Converters.Add(new DataFlowConverter());
            options.Converters.Add(new DateTimeConverter());
            options.Converters.Add(new DependencyConverter());
            options.Converters.Add(new ExternalReferenceTypeConverter());
            options.Converters.Add(new HashAlgorithmConverter());
            options.Converters.Add(new IssueClassificationConverter());
            options.Converters.Add(new LicenseConverter());
            options.Converters.Add(new PatchClassificationConverter());
        }
    }
}