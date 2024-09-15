using Devshift.NewsBoardManagement.Api.Shared.Models;

namespace Devshift.NewsBoardManagement.Api.Models
{
    public class AppSettings
    {
        public string VaultHost {get; set;}
        public string ContentManager {get; set;}
        public CredentialSetting CredentialSetting { get; set; }
    }
}