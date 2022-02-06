using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace OpenFaas.Secrets
{
    public interface IOpenFaasSecretsConfigurationProvider
    {
        T GetSecret<T>(string secretName) where T : class;
        string GetSecretJson(string secretName);
    }
    public class OpenFaasSecretsConfigurationProvider : IOpenFaasSecretsConfigurationProvider
    {
        private Dictionary<string, string> _secrets = null;
        private static readonly string SECRETS_PATH = "/var/openfaas/secrets/";
        private static readonly string SECRETS_PREFIX = "_secret_";
        private readonly ILogger _log;
        public OpenFaasSecretsConfigurationProvider(ILogger<OpenFaasSecretsConfigurationProvider> log)
        {
            _secrets = new Dictionary<string, string>();
            _log = log;
        }
        public T GetSecret<T>(string secretName) where T : class
        {
            Build();
            _log.LogInformation($"Loading {secretName}");
            string name = string.Concat(SECRETS_PREFIX, secretName);
            T secret = default;
            string json = "";
            if (_secrets.TryGetValue(name, out json))
            {
                _log.LogInformation($"Found secret with name {secretName}.  value: {json}");
                secret = string.IsNullOrEmpty(json) ? default(T) : JsonConvert.DeserializeObject<T>(json);
            }
            return secret;

        }

        public string GetSecretJson(string secretName)
        {
            Build();
            _log.LogInformation($"Loading {secretName}");
            string name = string.Concat(SECRETS_PREFIX, secretName);            
            string json = "";
            var b = _secrets.TryGetValue(name, out json);            
            return json;

        }

        private void Build()
        {
            if (_secrets != null) return;

            if (!Directory.Exists(SECRETS_PATH))
            {
                return;
            }

            try
            {
                var secretFiles = Directory.GetFiles(SECRETS_PATH);

                foreach (var secretFile in secretFiles)
                {
                    _log.LogInformation($"Found secret file {secretFile}");
                    var secretName = string.Concat(SECRETS_PREFIX, Path.GetFileName(secretFile));
                    var secretValue = File.ReadAllBytes(secretFile);
                    var str = System.Text.Encoding.Default.GetString(secretValue);
                    _log.LogInformation($"Found secret file {secretFile}, value: {str}");
                    _secrets.TryAdd(secretName, str);
                }
            }
            catch (Exception ex)
            {
                _log.LogError("OpenFaasSecretsConfigurationProvider.Build", ex);
            }
        }
    }
}
