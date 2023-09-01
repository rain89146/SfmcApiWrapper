using System;
namespace SalesforceMarketingCloudIntegration.Models
{
	public class MarketingCloudClientConfigObject
    {
        public string? clientId { get; set; }
        public string? clientSecret { get; set; }
        public string? authEndPoint { get; set; }
        public string? restEndPoint { get; set; }
        public string? soapEndPoint { get; set; }
	}
}

