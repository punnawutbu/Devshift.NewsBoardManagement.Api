using System.Collections.Generic;
using System.Threading.Tasks;
using Devshift.NewsBoardManagement.Api.Shared.Models;
using Devshift.ResponseMessage;

namespace Devshift.NewsBoardManagement.Api.Shared.Facdes
{
    public interface INewsFacade
    {
        Task<ResponseMessage<IEnumerable<NewsData>>> GetNews();
        Task<ResponseMessage<NewsData>> GetNews(string uuid);
        Task<ResponseMessage<bool>> UploadNews(NewsData data);
        Task<ResponseMessage<NewsData>> UpdateNews(NewsData data);
        Task<ResponseMessage<bool>> DeleteNews(string uuid);
    }
}