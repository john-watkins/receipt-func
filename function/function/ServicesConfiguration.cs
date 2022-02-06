using AwsApi;
using GoogleApi.mail;
using Microsoft.Extensions.DependencyInjection;
using OpenFaas.Secrets;

namespace OpenFaaS
{
    public static class ServicesConfiguration
    {
        public static void AddFuncServices(this IServiceCollection services)
        {
            services.AddScoped<IAwsS3, AwsS3>();
            services.AddScoped<IMailRepo, MailRepo>(config =>
            {
                IOpenFaasSecretsConfigurationProvider provider = config.GetRequiredService<IOpenFaasSecretsConfigurationProvider>();
                
                IMailRepo repo = new MailRepo()
            });
        }
    }
}
