
namespace Devshift.NewsBoardManagement.Api.Shared.Models
{
    public class S3model
    {
        public string BucketName { get; set; }
        public string ObjectKey { get; set; }
        public string MediaType { get; set; }
        public byte[] FileContent { get; set; }
    }
}