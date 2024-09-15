using System.Threading.Tasks;
using Devshift.NewsBoardManagement.Api.Shared.Facdes;
using Devshift.NewsBoardManagement.Api.Shared.Models;
using Devshift.ResponseMessage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Devshift.NewsBoardManagement.Api.Controllers
{
    [ApiController]
    [Route("v1/[controller]")]
    public class NewsController : ControllerBase
    {
        private readonly ILogger<NewsController> _logger;
        private readonly INewsFacade _facades;
        public NewsController(ILogger<NewsController> logger, INewsFacade facades)
        {
            _logger = logger;
            _facades = facades;
        }
        [HttpGet("all")]
        public async Task<ActionResult> GetNews()
        {
            _logger.LogInformation("GET v1/news/all");
            var resp = await _facades.GetNews();
            if (resp.Message == Message.Success)
            {
                return Ok(resp);
            }
            return StatusCode(204);
        }
        [HttpGet("{uuid}")]
        public async Task<ActionResult> GetNews(string uuid)
        {
            _logger.LogInformation($"GET v1/news/{uuid}/");
            var resp = await _facades.GetNews(uuid);
            if (resp.Message == Message.Success)
            {
                return Ok(resp);
            }
            return StatusCode(204);
        }
        [HttpPost("upload")]
        public async Task<ActionResult> UploadNews([FromBody] NewsData data)
        {
            _logger.LogInformation("GET v1/news/upload");
            var resp = await _facades.UploadNews(data);
            if (resp.Message == Message.Success)
            {
                return StatusCode(201, resp);
            }
            return StatusCode(204);
        }
        [HttpPut("update")]
        public async Task<ActionResult> UpdateNews([FromBody] NewsData data)
        {
            _logger.LogInformation($"GET v1/news/update");
            var resp = await _facades.UpdateNews(data);
            if (resp.Message == Message.Updated)
            {
                return Ok(resp);
            }
            return StatusCode(204);
        }
        [HttpDelete("{uuid}/delete")]
        public async Task<ActionResult> DeleteNews(string uuid)
        {
            _logger.LogInformation($"GET v1/news/{uuid}/delete");
            var resp = await _facades.DeleteNews(uuid);
            if (resp.Message == Message.Deleted)
            {
                return Ok(resp);
            }
            return StatusCode(204);
        }
    }
}