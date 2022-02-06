using Microsoft.Extensions.Configuration;

namespace OpenFaaS
{
    internal static class OpenFaasSecretsConfigurationExtensions
    {
        public static IConfigurationBuilder AddOpenFaaSSecret(this IConfigurationBuilder builder,string secretName)
        {
            builder.AddJsonFile(source =>
             {
                 source.Path = $"/var/openfaas/secrets/{secretName}.json";
                 source.Optional = true;
                 source.ReloadOnChange = true;
             });

            return builder;
        }
    }
}
