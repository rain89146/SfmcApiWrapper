using System;
using SalesforceMarketingCloudIntegration;

namespace SalesforceMarketingCloudIntegration
{
	public interface IMarktingCloudRepoInterface
	{
        void TokenBroker(string _accessToken, string _restUrl);
        Task<SalesforceMarketingCloudAccessTokenObject> GetAccessToken();
        Task<DataExtensionSuccessResponse> InsertRowIntoDataExtensionAsync<ParamType>(string DataExtensionKeyId, ParamType rows);
        Task<DataExtensionSuccessResponse> UpsertRowIntoDataExtensionAsync<ParamType>(string DataExtensionKeyId, ParamType rows);
        Task<RetrieveRequestStatusResponse> RetrieveStatusOfRequest(string requestId);
    }
}

