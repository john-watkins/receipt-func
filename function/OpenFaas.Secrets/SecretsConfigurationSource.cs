using Microsoft.Extensions.Configuration;

namespace OpenFaas.Secrets
{
    internal class SecretsConfigurationSource : IConfigurationSource
    {
        public IConfigurationProvider Build( IConfigurationBuilder builder )
        {            
            return ( new SecretsConfigurationProvider(new Microsoft.Extensions.Configuration.Json.JsonConfigurationSource
            {
                 Path = "/var/openfaas/secrets/"
            }) );
        }
    }
}
