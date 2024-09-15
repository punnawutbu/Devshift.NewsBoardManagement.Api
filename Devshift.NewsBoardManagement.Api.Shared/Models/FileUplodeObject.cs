using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Devshift.NewsBoardManagement.Api.Shared.Models
{
    public class FileUplodeObject
    {
        [Required]
        public IFormFile File { get; set; }
        [Required]
        public string JobId { get; set; }
        [Required]
        public string ReportId { get; set; }
        [Required]
        public string CertificateType { get; set; }
        [Required]
        public string ObjectType { get; set; }
        [Required]
        public string FileName { get; set; }
    }
}