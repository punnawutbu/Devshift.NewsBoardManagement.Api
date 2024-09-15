using System.Collections.Generic;
using System.Threading.Tasks;
using Devshift.NewsBoardManagement.Api.Shared.Models;

namespace Devshift.NewsBoardManagement.Api.Shared.Repositories
{
    public interface IContentManagerRepository
    {
        Task<IEnumerable<NewsData>> GetNews();
        Task<NewsData> GetNews(string uuid);
        Task<IEnumerable<Images>> GetImages(int newsId);
        Task<string> UploadNews(NewsData data);
        Task<bool> UploadImage(IEnumerable<Images> images);
        Task<bool> UpdateNews(NewsData data);
        Task<bool> DeleteNews(string uuid);
        Task<bool> DeleteImages(int newsId);
        Task<int> GetNewsIdByUuid(string newsUuid);
    }
}