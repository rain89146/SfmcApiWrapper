using System;
namespace SalesforceMarketingCloudIntegration.Models
{
    public class ResultMessages
    {
        public string? resultType { get; set; }
        public string? resultClass { get; set; }
        public string? resultCode { get; set; }
        public string? message { get; set; }
    }

    public class DataExtensionErrorResponse
	{
        public string? requestId { get; set; }
        public List<ResultMessages>? resultMessages { get; set; }
    }
}

