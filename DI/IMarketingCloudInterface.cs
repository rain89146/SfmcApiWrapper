using System;
using SalesforceMarketingCloudIntegration.Models;

namespace SalesforceMarketingCloudIntegration.DI
{
	public interface IMarketingCloudInterface
	{
        Task<string> InsertRowAsync<ParamType>(string DeId, ParamType rows);
        Task<string> UpsertRowAsync<ParamType>(string DeId, ParamType rows);
        Task<bool> GetStatusOfAsyncRequest(string requestId);
        Task<List<ResultItem>> GetResultOfAsyncRequest(string requestId);
    }
}

