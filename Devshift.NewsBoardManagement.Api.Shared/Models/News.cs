
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;

namespace Devshift.NewsBoardManagement.Api.Shared.Models
{
    public class NewsData
    {
        [JsonIgnore]
        public int Id { get; set; }
        public string NewsUuid { get; set; }
        public string Title { get; set; }
        public string Content1 { get; set; }
        public string Content2 { get; set; }
        public string Content3 { get; set; }
        public List<Images> Images { get; set; }
    }
    public class Images
    {
        [JsonIgnore]
        public int NewsId { get; set; }
        public IFormFile File { get; set; }
        public string ImageUrl { get; set; }
        public string ImageUuid { get; set; }
        public string IsMain { get; set; }
    }
}