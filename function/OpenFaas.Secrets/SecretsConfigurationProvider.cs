using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Newtonsoft.Json;

namespace OpenFaas.Secrets
{
    internal class SecretsConfigurationProvider : JsonConfigurationProvider
    {        

        public SecretsConfigurationProvider(JsonConfigurationSource source) : base(source)
        {
        }

        public override void Load()
        {
            string path = Source.Path;
            if (!Directory.Exists(Source.Path))
            {
                Console.WriteLine($"SecretsConfigurationProvider: Path does not exist [{path}]");
                return;
            }

            try
            {
                var secrets = Directory.GetFiles(path);
                var secretFile = secrets.FirstOrDefault();
                if (string.IsNullOrEmpty(secretFile))
                {
                    Console.WriteLine($"SecretsConfigurationProvider: Secret file does not exist [{secretFile}]");
                    return;
                }
                Source.Path = secretFile;
                Console.WriteLine($"SecretsConfigurationProvider: Loading secret file [{secretFile}]");
                using (var stream = File.OpenRead(secretFile))
                {
                    base.Load(stream);
                }
                    
                Console.WriteLine($"SecretsConfigurationProvider: Configuration data hash: {JsonConvert.SerializeObject(Data)}");
                //var secretName = Path.GetFileName(secret);
                //var secretValue = File.ReadAllBytes(secret);
                //var str = System.Text.Encoding.Default.GetString(secretValue);
                //JsonConfigurationFileParser
                //    Console.WriteLine(str);
                //Data.Add(secretName, str);
                //foreach (var secret in secrets)
                //{
                    
                //}
            }
            catch (Exception)
            { }
        }
    }
}
