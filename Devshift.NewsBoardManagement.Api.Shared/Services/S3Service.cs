using System;
using System.IO;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Devshift.NewsBoardManagement.Api.Shared.Models;
using Microsoft.AspNetCore.Http;

namespace Devshift.NewsBoardManagement.Api.Shared.Services
{
    public class S3Service : IS3Service
    {
        private readonly AmazonS3Client _client;
        private readonly S3Setting _s3Setting;
        public S3Service(AmazonS3Client client, S3Setting s3Setting)
        {
            _client = client;
            _s3Setting = s3Setting;
        }
        public async Task<string> AmazonS3UploadAsync(string base64Image, string path)
        {

            using (var transferUtility = new Amazon.S3.Transfer.TransferUtility(_client))
            {

                byte[] imageBytes = Convert.FromBase64String(base64Image);
                using (var stream = new System.IO.MemoryStream(imageBytes))
                {
                    var key = _s3Setting.BasePath + "/" + path + "/" + Guid.NewGuid().ToString() + "." + this.GetFileExtensionFromBase64(base64Image);
                    await transferUtility.UploadAsync(stream, _s3Setting.BucketName, key);
                    return key;
                }
            }

        }

        public async Task<S3model> AmazonS3GetObjectAsync(string key)
        {

            var getObjectRequest = new GetObjectRequest
            {
                BucketName = _s3Setting.BucketName,
                Key = key
            };
            try
            {
                var response = await _client.GetObjectAsync(getObjectRequest);

                // Read the object's content into a byte array.
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    response.ResponseStream.CopyTo(memoryStream);
                    byte[] fileContent = memoryStream.ToArray();

                    // Get the content type (media type) of the object.
                    string mediaType = response.Headers.ContentType;

                    return new S3model
                    {
                        BucketName = _s3Setting.BucketName,
                        ObjectKey = key,
                        MediaType = mediaType,
                        FileContent = fileContent
                    };
                }
            }
            catch (AmazonS3Exception ex)
            {
                // Handle any exceptions, e.g., file not found, etc.
                Console.WriteLine("Error fetching object data: " + ex.Message);
                return null;
            }

        }


        private string GetFileExtensionFromBase64(string base64String)
        {
            try
            {
                // Remove the data URI scheme from the base64 string if present
                if (base64String.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
                {
                    var startIndex = base64String.IndexOf(',') + 1;
                    base64String = base64String.Substring(startIndex);
                }

                // Convert the base64 string to bytes
                byte[] imageBytes = Convert.FromBase64String(base64String);

                // Determine the file extension based on the magic bytes/signatures
                // You can add more file types and their magic bytes if needed.
                if (imageBytes.Length >= 2 && imageBytes[0] == 0xFF && imageBytes[1] == 0xD8)
                {
                    return "jpg";
                }
                else if (imageBytes.Length >= 3 && imageBytes[0] == 0x47 && imageBytes[1] == 0x49 && imageBytes[2] == 0x46)
                {
                    return "gif";
                }
                else if (imageBytes.Length >= 4 && imageBytes[0] == 0x89 && imageBytes[1] == 0x50 && imageBytes[2] == 0x4E && imageBytes[3] == 0x47)
                {
                    return "png";
                }
                else if (imageBytes.Length >= 2 && imageBytes[0] == 0x42 && imageBytes[1] == 0x4D)
                {
                    return "bmp";
                }
                else
                {
                    // Unable to determine the file extension; return an empty string or throw an exception as needed.
                    return string.Empty;
                }
            }
            catch (Exception)
            {
                // Error occurred while processing the base64 string; return an empty string or throw an exception as needed.
                return string.Empty;
            }
        }

        public async Task<string> AmazonS3UploadFormAsync(FileUplodeObject uploadObject)
        {
            var path = $"{uploadObject.FileName}";
            return await this.AmazonS3UploadByFormAsync(uploadObject.File, path);
        }

        private async Task<string> AmazonS3UploadByFormAsync(IFormFile file, string path)
        {
            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                stream.Seek(0, SeekOrigin.Begin);
                var transferUtility = new Amazon.S3.Transfer.TransferUtility(_client);
                var key = _s3Setting.BasePath + "/" + path.Replace(" ", "_").ToLower();
                await transferUtility.UploadAsync(stream, _s3Setting.BucketName, key);
                return key;
            }
        }
    }
}