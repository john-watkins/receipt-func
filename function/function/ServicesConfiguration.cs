using AwsApi;
using GoogleApi.mail;
using Microsoft.Extensions.DependencyInjection;

namespace OpenFaaS
{
    public static class ServicesConfiguration
    {
        public static void AddCustomServices(this IServiceCollection services)
        {
            services.AddScoped<IAwsS3, AwsS3>();
            services.AddScoped<IMailRepo, MailRepo>();
        }
    }
}
