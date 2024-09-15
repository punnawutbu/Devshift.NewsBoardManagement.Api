using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Devshift.NewsBoardManagement.Api.Shared.Models;
using Devshift.NewsBoardManagement.Api.Shared.Repositories;
using Devshift.NewsBoardManagement.Api.Shared.Services;
using Devshift.ResponseMessage;

namespace Devshift.NewsBoardManagement.Api.Shared.Facdes
{
    public class NewsFacade : INewsFacade
    {
        private readonly IContentManagerRepository _repository;
        private readonly IS3Service _s3Service;
        public NewsFacade(IContentManagerRepository repository, IS3Service s3Service)
        {
            _repository = repository;
            _s3Service = s3Service;
        }
        public async Task<ResponseMessage<IEnumerable<NewsData>>> GetNews()
        {
            var resp = new ResponseMessage<IEnumerable<NewsData>>
            {
                Message = Message.NoContent
            };

            var newsData = await _repository.GetNews();

            if (newsData == null || !newsData.Any())
            {
                return resp;
            }

            var newsResult = await _SetImage(newsData);

            resp.Message = Message.Success;
            resp.Result = newsResult;

            return resp;
        }
        public async Task<ResponseMessage<NewsData>> GetNews(string uuid)
        {
            var resp = new ResponseMessage<NewsData>
            {
                Message = Message.NoContent,
                Result = new NewsData()
            };

            var newsData = await _repository.GetNews(uuid);

            if (newsData == null)
            {
                return resp;
            }

            var newsResult = await _SetImage(newsData);

            resp.Message = Message.Success;
            resp.Result = newsResult;

            return resp;
        }
        public async Task<ResponseMessage<bool>> UploadNews(NewsData data)
        {
            var resp = new ResponseMessage<bool>
            {
                Message = Message.Fail,
                Result = false
            };

            string newsUuid = await _repository.UploadNews(data);
            if (string.IsNullOrEmpty(newsUuid))
            {
                return resp;
            }
            data.NewsUuid = newsUuid;
            await _repository.UploadImage(data.Images);

            await _UploadImagesToS3(data);

            resp.Message = Message.Success;
            resp.Result = true;

            return resp;
        }
        public async Task<ResponseMessage<NewsData>> UpdateNews(NewsData data)
        {
            var resp = new ResponseMessage<NewsData>
            {
                Message = Message.Fail
            };

            await _repository.UpdateNews(data);
            int newsId = await _repository.GetNewsIdByUuid(data.NewsUuid);
            await _repository.DeleteImages(newsId);
            await _repository.UploadImage(data.Images);
            var newsData = await GetNews(data.NewsUuid);

            resp.Message = Message.Updated;
            resp.Result = newsData.Result;

            return resp;
        }
        public async Task<ResponseMessage<bool>> DeleteNews(string uuid)
        {
            var resp = new ResponseMessage<bool>
            {
                Message = Message.Fail,
                Result = false
            };

            await _repository.DeleteNews(uuid);
            int newsId = await _repository.GetNewsIdByUuid(uuid);
            await _repository.DeleteImages(newsId);

            resp.Message = Message.Deleted;
            resp.Result = true;

            return resp;
        }

        #region  Private Methods

        private async Task<T> _SetImage<T>(T data) where T : class
        {
            if (data is IEnumerable<NewsData> newsList)
            {
                foreach (var news in newsList)
                {
                    await _SetImageForNews(news);
                }
            }
            else if (data is NewsData newsData)
            {
                await _SetImageForNews(newsData);
            }

            return data;
        }
        private async Task _SetImageForNews(NewsData newsData)
        {
            var imagesList = await _repository.GetImages(newsData.Id);
            newsData.Images = imagesList.ToList();
        }
        private async Task<IEnumerable<string>> _UploadImagesToS3(NewsData data)
        {
            var uploadPaths = new List<string>();

            // วนลูปผ่าน IEnumerable<Images> เพื่ออัปโหลดแต่ละไฟล์
            foreach (var image in data.Images)
            {
                // ตรวจสอบว่าไฟล์มีอยู่จริงก่อนที่จะทำการอัปโหลด
                if (image?.File != null && image.File.Length > 0)
                {
                    string fileName = $"{data.NewsUuid}-{image.ImageUuid}"; // สร้างชื่อไฟล์

                    // เรียกใช้งานฟังก์ชันสำหรับอัปโหลดไฟล์
                    var path = await _s3Service.AmazonS3UploadFormAsync(new FileUplodeObject
                    {
                        FileName = fileName,
                        File = image.File, // ไฟล์จาก IFormFile
                        ObjectType = image.File.ContentType // ContentType ของไฟล์
                    });

                    // เพิ่ม path ที่อัปโหลดเสร็จแล้วลงในรายการ
                    uploadPaths.Add(path);
                }
            }

            return uploadPaths;
        }

        #endregion
    }
}