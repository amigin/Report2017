using Lykke.Sdk.Settings;

namespace Report2016
{
	public class AuthenticationModel
	{
		public string ClientId { get; set; }
		public string ClientSecret { get; set; }
		public string PostLogoutRedirectUri { get; set; }
		public string Authority { get; set; }
	}

    public class ReportsModel
    {
        public string VotesConnectionString { get; set; }
        public string LogsConnectionString { get; set; }

        public AuthenticationModel Authentication { get; set; }
    }

    public class SettingsModel : BaseAppSettings
    {
        public ReportsModel Report2016 { get; set; }
    }
}
