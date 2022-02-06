using Microsoft.Extensions.Configuration;
using OpenFaaS.Configuration;

namespace OpenFaas.Secrets
{
    internal static class SecretsConfigurationExtensions
    {
        public static IConfigurationBuilder AddOpenFaaSSecrets( this IConfigurationBuilder configurationBuilder )
        {
            configurationBuilder.Add( new SecretsConfigurationSource() );

            return ( configurationBuilder );
        }
    }
}
