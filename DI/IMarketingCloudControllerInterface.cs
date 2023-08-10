using System;
namespace SalesforceMarketingCloudIntegration
{
	public interface IMarketingCloudControllerInterface
	{
        //  DE async
        Task ServiceInit();
        Task<string> InsertRowAsync<ParamType>(string DataExtensionKeyId, ParamType rows);
        Task<string> UpsertRowAsync<ParamType>(string DataExtensionKeyId, ParamType rows);
        Task<bool> GetStatusOfAsyncRequest(string requestId);
    }
}

