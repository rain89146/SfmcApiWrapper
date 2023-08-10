using System;

namespace SalesforceMarketingCloudIntegration
{
	public class MarketingCloudController: IMarketingCloudControllerInterface
    {
		private readonly IMarktingCloudRepoInterface _repo;
        
        //
		public MarketingCloudController(IMarktingCloudRepoInterface repo)
		{
			this._repo = repo;
		}

        //
        public async Task ServiceInit()
        {
            //  get access token
            var response = await this._repo.GetAccessToken();

            //
            _repo.TokenBroker(response.access_token, response.rest_instance_url);
        }

        //
        public async Task<string> InsertRowAsync<ParamType>(string DataExtensionKeyId, ParamType rows)
        {
            //
            var response = await this._repo.InsertRowIntoDataExtensionAsync(DataExtensionKeyId, rows);

            //
            return response.requestId;
        }

        //
        public async Task<string> UpsertRowAsync<ParamType>(string DataExtensionKeyId, ParamType rows)
        {
            //
            var response = await this._repo.UpsertRowIntoDataExtensionAsync(DataExtensionKeyId, rows);

            //
            return response.requestId;
        }

        //
        public async Task<bool> GetStatusOfAsyncRequest(string requestId)
        {
            //
            var response = await this._repo.RetrieveStatusOfRequest(requestId);

            //
            return response.status.hasErrors == false && response.status.resultStatus == "OK";
        }
    }
}

