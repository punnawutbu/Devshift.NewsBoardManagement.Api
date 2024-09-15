using System.Threading.Tasks;
using Devshift.NewsBoardManagement.Api.Shared.Models;

namespace Devshift.NewsBoardManagement.Api.Shared.Services
{
    public interface IS3Service
    {
        Task<string> AmazonS3UploadAsync(string base64Image, string path);
        Task<string> AmazonS3UploadFormAsync(FileUplodeObject uploadObject);
        Task<S3model> AmazonS3GetObjectAsync(string key);
    }
}