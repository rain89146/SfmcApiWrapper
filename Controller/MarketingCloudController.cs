using System;
using SalesforceMarketingCloudIntegration.Models;
using SalesforceMarketingCloudIntegration.Helper;
using SalesforceMarketingCloudIntegration.DI;


namespace SalesforceMarketingCloudIntegration
{
	public class MarketingCloudController: IMarketingCloudControllerInterface
    {
        //
		private readonly IMarktingCloudRepoInterface _repo;

        //
        private readonly int maxRetries = 5;
        private readonly int retriesOffsetTimeMS = 500;
        
        //
		public MarketingCloudController(IMarktingCloudRepoInterface repo)
		{
			this._repo = repo;
		}

        //
        public async Task<string> InsertRowAsync<ParamType>(string DataExtensionKeyId, ParamType rows)
        {
            var response = await Tools.DoRetryAsync(
                async() => await _repo.InsertRowIntoDataExtensionAsync(DataExtensionKeyId, rows),
                TimeSpan.FromMilliseconds(retriesOffsetTimeMS),
                maxRetries
                );

            return response.requestId;
        }

        //
        public async Task<string> UpsertRowAsync<ParamType>(string DataExtensionKeyId, ParamType rows)
        {
            var response = await Tools.DoRetryAsync(
                async () => await this._repo.UpsertRowIntoDataExtensionAsync(DataExtensionKeyId, rows),
                TimeSpan.FromMilliseconds(retriesOffsetTimeMS),
                maxRetries
                );

            return response.requestId;
        }

        //
        public async Task<bool> GetStatusOfAsyncRequest(string requestId)
        {
            var response = await Tools.DoRetryAsync(
                async () => await _repo.RetrieveStatusOfRequest(requestId),
                TimeSpan.FromMilliseconds(retriesOffsetTimeMS),
                maxRetries
                );

            if (response is null || response.status is null) return false;

            return response.status.hasErrors == false && response.status.resultStatus == "OK";
        }

        //
        public async Task<List<ResultItem>> GetResultOfAsyncRequest(string requestId)
        {
            var response = await Tools.DoRetryAsync(
                async () => await this._repo.RetrieveResultOfRequest(requestId),
                TimeSpan.FromMilliseconds(retriesOffsetTimeMS),
                maxRetries
                );

            if (response is null || response.items is null) return new List<ResultItem>();

            return response.items;
        }
    }
}

