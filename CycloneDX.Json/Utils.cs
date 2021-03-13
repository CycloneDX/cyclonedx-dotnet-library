using System.Text.Json;
using CycloneDX.Json;

namespace CycloneDX.Json
{
    public static class Utils
    {
        public static JsonSerializerOptions GetJsonSerializerOptions_v1_3()
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                IgnoreNullValues = true,
            };
            options.Converters.Add(new v1_3.Converters.ComponentTypeConverter());
            options.Converters.Add(new v1_3.Converters.DataFlowConverter());
            options.Converters.Add(new v1_3.Converters.DateTimeConverter());
            options.Converters.Add(new v1_3.Converters.DependencyConverter());
            options.Converters.Add(new v1_3.Converters.ExternalReferenceTypeConverter());
            options.Converters.Add(new v1_3.Converters.HashAlgorithmConverter());
            options.Converters.Add(new v1_3.Converters.IssueClassificationConverter());
            options.Converters.Add(new v1_3.Converters.LicenseConverter());
            options.Converters.Add(new v1_3.Converters.PatchClassificationConverter());
            return options;
        }

        public static JsonSerializerOptions GetJsonSerializerOptions_v1_2()
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                IgnoreNullValues = true,
            };
            options.Converters.Add(new v1_2.Converters.ComponentTypeConverter());
            options.Converters.Add(new v1_2.Converters.DataFlowConverter());
            options.Converters.Add(new v1_2.Converters.DateTimeConverter());
            options.Converters.Add(new v1_2.Converters.DependencyConverter());
            options.Converters.Add(new v1_2.Converters.ExternalReferenceTypeConverter());
            options.Converters.Add(new v1_2.Converters.HashAlgorithmConverter());
            options.Converters.Add(new v1_2.Converters.IssueClassificationConverter());
            options.Converters.Add(new v1_2.Converters.LicenseConverter());
            options.Converters.Add(new v1_2.Converters.PatchClassificationConverter());
            return options;
        }
    }
}