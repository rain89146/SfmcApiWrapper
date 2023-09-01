using System.Net;
using System.Text;
using Newtonsoft.Json;
using SalesforceMarketingCloudIntegration.Exceptions;
using SalesforceMarketingCloudIntegration.Models;
using SalesforceMarketingCloudIntegration.DI;

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

        /// <summary>
        /// Get access token
        /// </summary>
        /// <returns></returns>
        /// <exception cref="HttpsFailedException"></exception>
        /// <exception cref="UnableToRetrieveSalesforceMarketingCloudAccessToken"></exception>
        private async Task<SalesforceMarketingCloudAccessTokenObject> GetAccessToken()
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

            //
            string responseContent = await response.Content.ReadAsStringAsync();

            //	Http request failed
            if (!response.IsSuccessStatusCode) HttpRequestFailedHandler(response);

            //
            if (string.IsNullOrEmpty(responseContent)) throw new UnableToRetrieveSalesforceMarketingCloudAccessToken();

            //
            var result = JsonConvert.DeserializeObject<SalesforceMarketingCloudAccessTokenObject>(responseContent);

            //
            return result ?? throw new UnableToRetrieveSalesforceMarketingCloudAccessToken();
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
            await SetAuthoriziationHeader();

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

            //  get response string
            string responseContent = await response.Content.ReadAsStringAsync();

            //  if it's unauthorized, get authorized and try again
            if (response.StatusCode == HttpStatusCode.Unauthorized) await HttpRequestUnauthorizedHandler(async () => await InsertRowIntoDataExtensionAsync(DataExtensionKeyId, rows));

            //	if it's salesforce error
            if (response.StatusCode == HttpStatusCode.BadRequest) SalesforceDataExtensionErrorResponse(responseContent);

            //	Http request failed
            if (!response.IsSuccessStatusCode) HttpRequestFailedHandler(response);

            //  no response string
            if (string.IsNullOrEmpty(responseContent)) throw new UnableToProcessDataExtensionRequest("Missing response content");

            //  parse response content
            var parsedResult = JsonConvert.DeserializeObject<DataExtensionSuccessResponse>(responseContent);

            //	parse into object
            return parsedResult ?? throw new UnableToProcessDataExtensionRequest("Can't parse response content");
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
            await SetAuthoriziationHeader();

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

            //  get response string
            string responseContent = await response.Content.ReadAsStringAsync();

            //  if it's unauthorized
            if (response.StatusCode == HttpStatusCode.Unauthorized) await HttpRequestUnauthorizedHandler(async () => await UpsertRowIntoDataExtensionAsync(DataExtensionKeyId, rows));

            //	if it's salesforce error
            if (response.StatusCode == HttpStatusCode.BadRequest) SalesforceDataExtensionErrorResponse(responseContent);

            //	Http request failed
            if (!response.IsSuccessStatusCode) HttpRequestFailedHandler(response);

            //  response content is null
            if (string.IsNullOrEmpty(responseContent)) throw new UnableToProcessDataExtensionRequest("Missing response content");

            //  parse response content
            var parsedResult = JsonConvert.DeserializeObject<DataExtensionSuccessResponse>(responseContent);

            //	parse into object
            return parsedResult ?? throw new UnableToProcessDataExtensionRequest("Can't parse response content");
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
            await SetAuthoriziationHeader();

            //	build request url
            var requestUrl = new Uri($"{this._restUrl}data/v1/async/{requestId}/status");

            //	make request
            using HttpResponseMessage response = await this._httpClient.GetAsync(requestUrl);

            //
            string responseContent = await response.Content.ReadAsStringAsync();

            //  when it's unauthorized
            if (response.StatusCode == HttpStatusCode.Unauthorized) await HttpRequestUnauthorizedHandler(async () => await RetrieveStatusOfRequest(requestId));

            //  when it's failed
            if (!response.IsSuccessStatusCode) HttpRequestFailedHandler(response);

            //
            var responseObject = JsonConvert.DeserializeObject<RetrieveRequestStatusResponse>(responseContent);

            //  means the request failed
            if (responseObject!.status is null) SalesforceDataExtensionErrorResponse(responseContent);

            //
            return responseObject;
        }

        /// <summary>
        /// Retrieve result of request
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns></returns>
        /// <exception cref="HttpsFailedException"></exception>
        /// <exception cref="UnableToProcessDataExtensionRequest"></exception>
        public async Task<ResultAsyncRequestResponse> RetrieveResultOfRequest(string requestId)
        {
            //
            await SetAuthoriziationHeader();

            //
            var requestUrl = new Uri($"{this._restUrl}data/v1/async/{requestId}/results");

            //
            using HttpResponseMessage response = await this._httpClient.GetAsync(requestUrl);

            //
            string responseContent = await response.Content.ReadAsStringAsync();

            //  when it's unauthorized
            if (response.StatusCode == HttpStatusCode.Unauthorized) await HttpRequestUnauthorizedHandler(async () => await this.RetrieveResultOfRequest(requestId));

            //  when it's failed
            if (!response.IsSuccessStatusCode) HttpRequestFailedHandler(response);

            //
            var responseObject = JsonConvert.DeserializeObject<ResultAsyncRequestResponse>(responseContent) ?? throw new UnableToProcessDataExtensionRequest("Response object is null");

            //  request success
            if (responseObject.items is not null && responseObject.items.Count > 0) return responseObject;

            //  error presented
            if (responseObject.resultMessages is null || responseObject.resultMessages.Count == 0) throw new UnableToProcessDataExtensionRequest("Result message empty");

            //
            var messages = new StringBuilder();

            //
            foreach(var messageObj in responseObject!.resultMessages) messages.Append($"Error {messageObj.resultCode}, message: {messageObj.message};");

            //
            throw new UnableToProcessDataExtensionRequest(messages.ToString());
        }

        /// <summary>
        /// set authorization header
        /// </summary>
        /// <param name="accessToken"></param>
        /// <exception cref="MissingAccessTokenException"></exception>
        private async Task SetAuthoriziationHeader()
        {
            //	access token validation
            if (string.IsNullOrWhiteSpace(this._accessToken)) await Reauthorize();
      
            //  clear access token
            this._httpClient.DefaultRequestHeaders.Remove("Authorization");

            //	add bearer token to header
            this._httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {this._accessToken}");
        }

        /// <summary>
        /// When it's unauthorized, try to get access token again, then assign the token.
        /// </summary>
        /// <returns></returns>
        private async Task Reauthorize()
        {
            //  get the access token
            var tokenObj = await GetAccessToken();

            //  reassign
            this._accessToken = tokenObj.access_token;
            this._restUrl = tokenObj.rest_instance_url;
        }

        /// <summary>
        /// Data extension error handling
        /// </summary>
        /// <param name="responsString"></param>
        /// <exception cref="UnableToProcessDataExtensionRequest"></exception>
        private static void SalesforceDataExtensionErrorResponse(string responsString)
        {
            //	
            var errorObject = JsonConvert.DeserializeObject<DataExtensionErrorResponse>(responsString) ?? throw new UnableToProcessDataExtensionRequest("Error object is null");

            //
            if (errorObject.resultMessages is null) throw new UnableToProcessDataExtensionRequest("Error result message is empty");

            //	more than 1 error object in the message
            List<ResultMessages> errorList = errorObject.resultMessages;

            //  
            if (errorList.Count != 0) throw new UnableToProcessDataExtensionRequest("");

            //	
            var message = new StringBuilder();

            //
            foreach (ResultMessages error in errorList) message.Append($"Error {error.resultCode}: {error.message};");

            //  
            throw new UnableToProcessDataExtensionRequest(message.ToString());
        }

        /// <summary>
        /// When http request failed because of the status is unauthorized
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="response"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        private async Task HttpRequestUnauthorizedHandler<TaskType>(Func<Task<TaskType>> callback)
        {
            await Reauthorize();
            await callback();
        }

        /// <summary>
        /// When http request failed
        /// </summary>
        /// <param name="response"></param>
        /// <exception cref="HttpsFailedException"></exception>
        private static void HttpRequestFailedHandler(HttpResponseMessage response)
        {
            //  when it's failed
            if (!response.IsSuccessStatusCode)
            {
                //
                if (response is null || response.ReasonPhrase is null) throw new HttpsFailedException("Failed with no reason", 500);

                //
                throw new HttpsFailedException(response.ReasonPhrase, (int)response.StatusCode);
            }
        }
    }
}

