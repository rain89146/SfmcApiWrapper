using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace SalesforceMarketingCloudIntegration
{
	public class MarketingCloudRepo: IMarktingCloudRepoInterface
    {
		//
		private readonly HttpClient _httpClient;
		private readonly MarketingCloudClientConfigObject _credential;
        private string? _accessToken;
        private string? _restUrl;

        //	Constructor
        public MarketingCloudRepo(HttpClient httpClient, MarketingCloudClientConfigObject credential)
		{
			this._httpClient = httpClient;
			this._credential = credential;
        }

		//  Get access token
		public async Task<SalesforceMarketingCloudAccessTokenObject> GetAccessToken()
		{
			//
            string json = JsonConvert.SerializeObject(
				new {
					grant_type = "client_credentials",
					client_id = this._credential.clientId,
					client_secret = this._credential.clientSecret,
				}
			);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

			//
			var requestUrl = new Uri($"{this._credential.authEndPoint}/v2/token");

            //
            using var response = await this._httpClient.PostAsync(requestUrl, httpContent).ConfigureAwait(false);

			//	Http request failed
			if (!response.IsSuccessStatusCode) throw new HttpsFailedException(response.ReasonPhrase, (int) response.StatusCode);

            //
            string responseContent = await response.Content.ReadAsStringAsync();

            //
            if (string.IsNullOrEmpty(responseContent)) throw new UnableToRetrieveSalesforceMarketingCloudAccessToken();

			//
			return JsonConvert.DeserializeObject<SalesforceMarketingCloudAccessTokenObject>(responseContent);
		}

        //  assign access token to this
        public void TokenBroker (string _accessToken, string _restUrl)
        {
            this._accessToken = _accessToken;
            this._restUrl = _restUrl;
        }


        /// <summary>
        /// Insert row into data extension
        /// </summary>
        /// <typeparam name="ParamType"></typeparam>
        /// <param name="DataExtensionKeyId"></param>
        /// <param name="rows"></param>
        /// <returns>DataExtensionSuccessResponse</returns>
        /// <exception cref="MissingAccessTokenException"></exception>
        /// <exception cref="HttpsFailedException"></exception>
        /// <exception cref="UnableToProcessDataExtensionRequest"></exception>
        public async Task<DataExtensionSuccessResponse> InsertRowIntoDataExtensionAsync<ParamType>(string DataExtensionKeyId, ParamType rows)
		{
            //	add bearer token to header
            this.SetAuthoriziationHeader(this._accessToken);

            //	compose payload string
            string json = JsonConvert.SerializeObject(
				new
				{
                    items = rows
                }
			);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            //	build request url
            var requestUrl = new Uri($"{this._restUrl}data/v1/async/dataextensions/key:{DataExtensionKeyId}/rows");

            //	make request
            using var response = await this._httpClient.PostAsync(requestUrl, httpContent);

            //  if it's unauthorized
            if (response.StatusCode == HttpStatusCode.Unauthorized) await UnauthorizeResponse(async () => await InsertRowIntoDataExtensionAsync(DataExtensionKeyId, rows));

            //	if it's salesforce error
            if ((int) response.StatusCode == 400 ) await this.SalesforceDataExtensionErrorResponse(response);

            //	Http request failed
            if (!response.IsSuccessStatusCode) throw new HttpsFailedException(response.ReasonPhrase, (int)response.StatusCode);
           
            //  get response string
            string responseContent = await response.Content.ReadAsStringAsync();

            //  no response string
            if (string.IsNullOrEmpty(responseContent)) throw new UnableToProcessDataExtensionRequest("Missing response content");

            //	parse into object
            return JsonConvert.DeserializeObject<DataExtensionSuccessResponse>(responseContent);
        }


        /// <summary>
        /// Upsert row into data extension
        /// </summary>
        /// <typeparam name="ParamType"></typeparam>
        /// <param name="DataExtensionKeyId"></param>
        /// <param name="rows"></param>
        /// <returns>DataExtensionSuccessResponse</returns>
        /// <exception cref="MissingAccessTokenException"></exception>
        /// <exception cref="HttpsFailedException"></exception>
        /// <exception cref="UnableToProcessDataExtensionRequest"></exception>
        public async Task<DataExtensionSuccessResponse> UpsertRowIntoDataExtensionAsync<ParamType>(string DataExtensionKeyId, ParamType rows)
		{
            //  add bearer token to header
            this.SetAuthoriziationHeader(this._accessToken);

            //	compose payload string
            string json = JsonConvert.SerializeObject(
                new
                {
                    items = rows
                }
            );
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            //	build request url
            var requestUrl = new Uri($"{this._restUrl}data/v1/async/dataextensions/key:{DataExtensionKeyId}/rows");

            //	make request
            using var response = await this._httpClient.PutAsync(requestUrl, httpContent);

            //  if it's unauthorized
            if (response.StatusCode == HttpStatusCode.Unauthorized) await UnauthorizeResponse(async () => await UpsertRowIntoDataExtensionAsync(DataExtensionKeyId, rows));

            //	if it's salesforce error
            if ((int)response.StatusCode == 400) await this.SalesforceDataExtensionErrorResponse(response);

            //	Http request failed
            if (!response.IsSuccessStatusCode) throw new HttpsFailedException(response.ReasonPhrase, (int)response.StatusCode);

            //  get response string
            string responseContent = await response.Content.ReadAsStringAsync();

            //
            if (string.IsNullOrEmpty(responseContent)) throw new UnableToProcessDataExtensionRequest("Missing response content");

            //	parse into object
            return JsonConvert.DeserializeObject<DataExtensionSuccessResponse>(responseContent);
        }


        /// <summary>
        /// Retrieve status of a request
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns>RetrieveRequestStatusResponse</returns>
        /// <exception cref="MissingAccessTokenException"></exception>
        /// <exception cref="HttpsFailedException"></exception>
        public async Task<RetrieveRequestStatusResponse> RetrieveStatusOfRequest(string requestId)
        {
            //	add bearer token to header
            this.SetAuthoriziationHeader(this._accessToken);

            //	build request url
            Uri requestUrl = new($"{this._restUrl}data/v1/async/{requestId}/status");

            //	make request
            using HttpResponseMessage response = await this._httpClient.GetAsync(requestUrl);

            //  when it's unauthorized
            if (response.StatusCode == HttpStatusCode.Unauthorized) await UnauthorizeResponse(async () => await this.RetrieveStatusOfRequest(requestId));

            //  when it's failed
            if (!response.IsSuccessStatusCode) throw new HttpsFailedException(response.ReasonPhrase, (int)response.StatusCode);

            //
            string responseContent = await response.Content.ReadAsStringAsync();

            //
            var responseObject = JsonConvert.DeserializeObject<RetrieveRequestStatusResponse>(responseContent);

            //  means the request failed
            if (responseObject.status is null) await this.SalesforceDataExtensionErrorResponse(response);

            //
            return responseObject;
        }

        //  set authorization header
        private void SetAuthoriziationHeader(string? accessToken)
        {
            //	access token validation
            if (string.IsNullOrWhiteSpace(accessToken)) throw new MissingAccessTokenException();

            //	add bearer token to header
            this._httpClient.DefaultRequestHeaders.Remove("Authorization");
            this._httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
        }

        /**
         * When it's unauthorized, try to get access token again
         * then assign the token, 
         * then execute the call again
         */
        private async Task UnauthorizeResponse(Func<Task> callbackFunction)
        {
            //  get the access token
            var tokenObj = await this.GetAccessToken();

            //  reassign
            TokenBroker(tokenObj.access_token, tokenObj.rest_instance_url);

            //  return
            await callbackFunction();
        }

        //  Data extension error handling
        private async Task SalesforceDataExtensionErrorResponse(HttpResponseMessage response)
        {
            //	error returns different structure
            string errorResponsString = await response.Content.ReadAsStringAsync();

            //	
            var errorObject = JsonConvert.DeserializeObject<DataExtensionErrorResponse>(errorResponsString);

            //	more than 1 error object in the message
            List<ResultMessages> errorList = errorObject.resultMessages;

            //	
            var message = new StringBuilder();

            //
            foreach (ResultMessages error in errorList) message.Append($"Error {error.resultCode}: {error.message};");

            //
            string errorMessage = (errorList.Count() == 0) ? "" : message.ToString();

            //
            throw new UnableToProcessDataExtensionRequest(errorMessage);
        }
    }
}

