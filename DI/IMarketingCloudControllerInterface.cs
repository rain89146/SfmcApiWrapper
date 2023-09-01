﻿using System;
using SalesforceMarketingCloudIntegration.Models;

namespace SalesforceMarketingCloudIntegration.DI
{
	public interface IMarketingCloudControllerInterface
	{
        //  DE async
        Task<string> InsertRowAsync<ParamType>(string DataExtensionKeyId, ParamType rows);
        Task<string> UpsertRowAsync<ParamType>(string DataExtensionKeyId, ParamType rows);
        Task<bool> GetStatusOfAsyncRequest(string requestId);
        Task<List<ResultItem>> GetResultOfAsyncRequest(string requestId);
    }
}

