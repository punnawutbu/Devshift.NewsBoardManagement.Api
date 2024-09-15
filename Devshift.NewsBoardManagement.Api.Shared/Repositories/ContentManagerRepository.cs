using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;
using Dapper;
using Devshift.Dapper;
using Devshift.Dapper.Models;
using Devshift.NewsBoardManagement.Api.Shared.Models;

namespace Devshift.NewsBoardManagement.Api.Shared.Repositories
{
    public class ContentManagerRepository : BaseRepository, IContentManagerRepository
    {
        public ContentManagerRepository(string connectionString, bool sslMode, Certificate certs) : base(connectionString, sslMode, certs)
        { }
        public async Task<IEnumerable<NewsData>> GetNews()
        {
            using (var sqlConnection = OpenDbConnection())
            {
                string sql = @"SELECT
                                id,
                                uuid,
                                title,
                                content1,
                                content2,
                                content3
                            FROM news";
                return await sqlConnection.QueryAsync<NewsData>(sql);
            }
        }
        public async Task<NewsData> GetNews(string uuid)
        {
            using (var sqlConnection = OpenDbConnection())
            {
                string sql = @"SELECT
                                id,
                                uuid,
                                title,
                                content1,
                                content2,
                                content3
                            FROM news
                            WHERE uuid::text = @Uuid";
                return await sqlConnection.QueryFirstOrDefaultAsync<NewsData>(sql, new { Uuid = uuid });
            }
        }
        public async Task<IEnumerable<Images>> GetImages(int newsId)
        {
            using (var sqlConnection = OpenDbConnection())
            {
                string sql = @"SELECT
                                image_url as ImageUrl,
                                is_main as IsMain
                            FROM images
                            WHERE news_id = @NewsId";
                return await sqlConnection.QueryAsync<Images>(sql, new { NewsId = newsId });
            }
        }
        public async Task<string> UploadNews(NewsData data)
        {
            using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                using (var sqlConnection = OpenDbConnection())
                {
                    string news = @"INSERT INTO public.news(
	                                title,
                                    content1,
                                    content2,
                                    content3,
                                    created_at)
	                            VALUES (
                                    Title,
                                    Content1,
                                    Content2,
                                    Content3,
                                    now())
                                RETURNING uuid;";
                    string newsUuid = await sqlConnection.ExecuteScalarAsync<string>(news, data);
                    if (string.IsNullOrEmpty(newsUuid))
                    {
                        return newsUuid;
                    }
                    transactionScope.Complete();
                    return newsUuid;
                }
            }
        }
        public async Task<bool> UploadImage(IEnumerable<Images> images)
        {
            using (var sqlConnection = OpenDbConnection())
            {
                string sql = @"INSERT INTO public.images(
                                news_id,
                                image_url,
                                is_main)
                            VALUES (
                                @NewsId,
                                @ImageUrl,
                                @IsMain);";
                await sqlConnection.ExecuteScalarAsync<int>(sql, images);
            }
            return true;
        }
        public async Task<bool> UpdateNews(NewsData data)
        {
            using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                using (var sqlConnection = OpenDbConnection())
                {
                    string news = @"UPDATE public.news
                                        SET
                                            title = @Title,
                                            content1 = @Content1,
                                            content2 = @Content2,
                                            content3 = @Content3,
                                            updated_at= now()
                                        WHERE uuid::text = @NewsUuid;";
                    int rowsAffected = await sqlConnection.ExecuteScalarAsync<int>(news, data);
                    if (rowsAffected == 0)
                    {
                        return false;
                    }
                }
                transactionScope.Complete();
                return true;
            }
        }
        public async Task<bool> DeleteNews(string uuid)
        {
            using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                using (var sqlConnection = OpenDbConnection())
                {
                    string news = @"DELETE FROM public.news WHERE uuid::text = @Uuid;";
                    int rowsAffected = await sqlConnection.ExecuteScalarAsync<int>(news, new { Uuid = uuid });
                    if (rowsAffected == 0)
                    {
                        return false;
                    }
                }
                transactionScope.Complete();
                return true;
            }
        }
        public async Task<bool> DeleteImages(int newsId)
        {
            using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                using (var sqlConnection = OpenDbConnection())
                {
                    string news = @"DELETE FROM public.images WHERE news_id = @NewsId;";
                    int rowsAffected = await sqlConnection.ExecuteScalarAsync<int>(news, new { NewsId = newsId });
                    if (rowsAffected == 0)
                    {
                        return false;
                    }
                }
                transactionScope.Complete();
                return true;
            }
        }
        public async Task<int> GetNewsIdByUuid(string newsUuid)
        {
            using (var sqlConnection = OpenDbConnection())
            {
                var query = @"SELECT id FROM public.news WHERE uuid::text = @NewsUuid";

                return await sqlConnection.QueryFirstOrDefaultAsync<int>(query, new { NewsUuid = newsUuid });
            }
        }
    }
}