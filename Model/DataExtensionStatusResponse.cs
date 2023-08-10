using System;
namespace SalesforceMarketingCloudIntegration
{
    public class RequestStatusObject
    {
        public string? callDateTime { get; set; }
        public string? completionDateTime { get; set; }
        public bool hasErrors { get; set; }
        public string? pickupDateTime { get; set; }
        public string? requestStatus { get; set; }
        public string? resultStatus { get; set; }
        public string? requestId { get; set; }
	}

	public class RetrieveRequestStatusResponse
	{
		public string? requestId { get; set; }
        public RequestStatusObject? status { get; set; }
        public List<ResultMessages>? resultMessages { get; set; }
    }
}

