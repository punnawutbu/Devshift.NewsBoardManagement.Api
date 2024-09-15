using Amazon.S3;
using Devshift.NewsBoardManagement.Api.Models;
using Devshift.NewsBoardManagement.Api.Shared.Facdes;
using Devshift.NewsBoardManagement.Api.Shared.Repositories;
using Devshift.NewsBoardManagement.Api.Shared.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Devshift.NewsBoardManagement.Api.Extensions
{
    public static class ServicesExtension
    {
        public static void ConfigureScopeFacades(this IServiceCollection services, AppSettings appSettings)
        {
            services.AddTransient<INewsFacade, NewsFacade>();
        }
        public static void ConfigureScopeService(this IServiceCollection services, AppSettings appSettings)
        {
            var credential = new Amazon.Runtime.BasicAWSCredentials(appSettings.CredentialSetting.S3Config.AccessKeyId, appSettings.CredentialSetting.S3Config.SecretAccessKey);
            var s3Client = new AmazonS3Client(credential, Amazon.RegionEndpoint.APSoutheast1);
            services.AddTransient<IS3Service, S3Service>(p => new S3Service(s3Client, appSettings.CredentialSetting.S3Config));
        }
        public static void ConfigureScopeRepository(this IServiceCollection services, AppSettings appSettings)
        {
            services.AddTransient<IContentManagerRepository, ContentManagerRepository>(s => new ContentManagerRepository(
            appSettings.CredentialSetting.ContentManagerConnectionString,
            appSettings.CredentialSetting.SslMode,
            appSettings.CredentialSetting.Certificate
            ));
        }
    }
}