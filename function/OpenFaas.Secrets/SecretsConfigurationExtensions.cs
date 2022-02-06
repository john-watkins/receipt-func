using Microsoft.Extensions.Configuration;

namespace OpenFaas.Secrets
{
    public static class SecretsConfigurationExtensions
    {
        public static IConfigurationBuilder AddOpenFaaSSecrets( this IConfigurationBuilder configurationBuilder )
        {
            configurationBuilder.Add( new SecretsConfigurationSource() );

            return ( configurationBuilder );
        }
    }
}
