using SalesforceMarketingCloudIntegration.Models;

namespace SalesforceMarketingCloudIntegration.DI
{
	public interface IDataExtensionRepoInterface
	{
        Task<DataExtensionSuccessResponse> InsertRowIntoDataExtensionAsync<ParamType>(string DataExtensionKeyId, ParamType rows);
        Task<DataExtensionSuccessResponse> UpsertRowIntoDataExtensionAsync<ParamType>(string DataExtensionKeyId, ParamType rows);
        Task<RetrieveRequestStatusResponse> RetrieveStatusOfRequest(string requestId);
        Task<ResultAsyncRequestResponse> RetrieveResultOfRequest(string requestId);
    }
}

