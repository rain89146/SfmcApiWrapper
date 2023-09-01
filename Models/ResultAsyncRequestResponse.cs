using System;
namespace SalesforceMarketingCloudIntegration.Models
{
    public class ResultItem
    {
        public string? errorCode { get; set; }
        public string? message { get; set; }
        public string? status { get; set; }
    }

    public class ResultMessage
    {
        public string? resultType { get; set; }
        public string? resultClass { get; set; }
        public string? resultCode { get; set; }
        public string? message { get; set; }
    }

    public class ResultAsyncRequestResponse
    {
		public int? page { get; set; }
        public int? pageSize { get; set; }
        public int? count { get; set; }
        public List<ResultItem>? items { get; set; }
        public List<ResultMessage>? resultMessages { get; set; }
    }
}

