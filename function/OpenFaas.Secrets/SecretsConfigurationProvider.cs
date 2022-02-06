using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace OpenFaas.Secrets
{
    internal class SecretsConfigurationProvider : ConfigurationProvider
    {
        private readonly string secretsPath = "/var/openfaas/secrets/";

        public override void Load()
        {
            if (!Directory.Exists(secretsPath))
            {
                return;
            }

            try
            {
                var secrets = Directory.GetFiles(secretsPath);

                foreach (var secret in secrets)
                {
                    var secretName = Path.GetFileName(secret);
                    var secretValue = File.ReadAllBytes(secret);
                    var str = System.Text.Encoding.Default.GetString(secretValue);
                    Data.Add(secretName, str);
                }
            }
            catch (Exception)
            { }
        }
    }
}
