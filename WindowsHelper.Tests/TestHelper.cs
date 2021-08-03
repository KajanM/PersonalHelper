using Microsoft.Extensions.Configuration;

namespace WindowsHelper.Tests
{
    public static class TestHelper
    {
        public static TSettings GetFromAppSettings<TSettings>(string key)
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false);

            var rootConfig = builder.Build().GetSection("App");

            return rootConfig.GetSection(key).Get<TSettings>();
        }
    }
}