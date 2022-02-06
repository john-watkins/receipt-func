using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace OpenFaas.Secrets
{
    internal class SecretsConfigurationSource : IConfigurationSource
    {
        public IConfigurationProvider Build( IConfigurationBuilder builder )
        {
            return ( new SecretsConfigurationProvider() );
        }
    }
}
