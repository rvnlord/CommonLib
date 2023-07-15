using CommonLib.Source.Common.Converters;
using CommonLib.Source.Common.Utils;
using Microsoft.Extensions.Configuration;

namespace CommonLib.Source.Common.Extensions
{
    public static class ConfigurationExtensions
    {
        public static AuthenticationStorageMode GetAuthenticationStorageMode(this IConfiguration configuration)
        {
            return configuration.GetSection("Authentication").GetValue<string>("StorageMode")?.ToEnum<AuthenticationStorageMode>() ?? AuthenticationStorageMode.NotSpecified;
        }  
    }
}
