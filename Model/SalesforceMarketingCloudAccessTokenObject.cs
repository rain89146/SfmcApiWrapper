using System;
namespace SalesforceMarketingCloudIntegration
{
	public class SalesforceMarketingCloudAccessTokenObject
	{
		public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
        public string scope { get; set; }
        public string soap_instance_url { get; set; }
        public string rest_instance_url { get; set; }
    }
}

