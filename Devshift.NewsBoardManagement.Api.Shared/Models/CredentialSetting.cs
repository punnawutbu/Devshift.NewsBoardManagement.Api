
using Devshift.Dapper.Models;

namespace Devshift.NewsBoardManagement.Api.Shared.Models
{
    public class CredentialSetting
    {
        public Certificate Certificate { get; set; }
        public string ContentManagerConnectionString { get; set; }
        public string SecertKey { get; set; }
        public string ServiceKey { get; set; }
        public bool SslMode { get; set; }
        public string RootPath { get; set; }
        public string FileUploadPath { get; set; }
        public string BasePath { get; set; }
        public string BucketName { get; set; }
        public S3Setting S3Config { get; set; }
    }
    public class S3Setting
    {
        public string AccessKeyId { get; set; }
        public string SecretAccessKey { get; set; }
        public string BucketName { get; set; }
        public string BasePath { get; set; }
    }
}