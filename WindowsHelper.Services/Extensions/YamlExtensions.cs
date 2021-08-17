using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace WindowsHelper.Services.Extensions
{
    public static class YamlExtensions
    {
        public static string SerializeToYaml<T>(this T @source) where T : class
        {
            var serializer = new SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            return serializer.Serialize(@source);
        }

        public static TOut DeserializeYaml<TOut>(this string yamlString, INamingConvention yamlNamingConvention = null)
        {
            yamlNamingConvention ??= CamelCaseNamingConvention.Instance;
            
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(yamlNamingConvention)
                .IgnoreUnmatchedProperties()
                .Build();
            
            return deserializer.Deserialize<TOut>(yamlString);
        }
    }
}