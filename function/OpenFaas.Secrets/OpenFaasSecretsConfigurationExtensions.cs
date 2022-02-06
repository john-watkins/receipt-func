using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace OpenFaas.Secrets
{
    public static class OpenFaasSecretsServiceExtensions
    {
        public static IServiceCollection AddOpenFaaSSecretService(this IServiceCollection services)
        {
            services.AddSingleton<IOpenFaasSecretsConfigurationProvider, OpenFaasSecretsConfigurationProvider>();
            return services;
        }
    }
}
