using System;
using Newtonsoft.Json;
using SalesforceMarketingCloudIntegration.Exceptions;
using System.Text;
using SalesforceMarketingCloudIntegration.Models;

namespace SalesforceMarketingCloudIntegration.Repo
{
	public class CloudBase
	{
        //
        protected readonly HttpClient _httpClient;
        protected readonly MarketingCloudClientConfigObject _credential;
        protected string? _accessToken;
        protected string? _restUrl;

        //
        public CloudBase(HttpClient httpClient, MarketingCloudClientConfigObject credential)
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
        protected async Task<SalesforceMarketingCloudAccessTokenObject> GetAccessToken()
        {
            //
            string json = JsonConvert.SerializeObject(
                new
                {
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
            if (!response.IsSuccessStatusCode)
            {
                //
                if (response is null) throw new HttpsFailedException("No response content presented", 500);

                //
                if (string.IsNullOrWhiteSpace(response.ReasonPhrase)) throw new HttpsFailedException("No response content presented", (int)response.StatusCode);
            }

            //
            if (string.IsNullOrEmpty(responseContent)) throw new UnableToRetrieveSalesforceMarketingCloudAccessToken();

            //
            var result = JsonConvert.DeserializeObject<SalesforceMarketingCloudAccessTokenObject>(responseContent);

            //
            return result ?? throw new UnableToRetrieveSalesforceMarketingCloudAccessToken();
        }

        /// <summary>
        /// Data extension error handling
        /// </summary>
        /// <param name="responsString"></param>
        /// <exception cref="UnableToProcessDataExtensionRequest"></exception>
        protected static void SalesforceDataExtensionErrorResponse(string responsString)
        {
            //	
            var errorObject = JsonConvert.DeserializeObject<DataExtensionErrorResponse>(responsString) ?? throw new UnableToProcessDataExtensionRequest("Error object is null");

            //
            if (errorObject.resultMessages is null) throw new UnableToProcessDataExtensionRequest("Error result message is empty");

            //	more than 1 error object in the message
            List<ResultMessages> errorList = errorObject.resultMessages;

            //  
            if (errorList.Count != 0) throw new UnableToProcessDataExtensionRequest("Error result is empty");

            //	
            StringBuilder message = new ();

            //
            foreach (ResultMessages error in errorList) message.Append($"Error {error.resultCode}: {error.message};");

            //  
            throw new UnableToProcessDataExtensionRequest(message.ToString());
        }

        /// <summary>
        /// When it's unauthorized, try to get access token again, then assign the token.
        /// </summary>
        /// <returns></returns>
        protected async Task Reauthorize()
        {
            //  get the access token
            var tokenObj = await GetAccessToken();

            //  reassign
            this._accessToken = tokenObj.access_token;
            this._restUrl = tokenObj.rest_instance_url;
        }

        /// <summary>
        /// Set authorization header;
        /// If the access token is null, reauthorize
        /// </summary>
        /// <param name="accessToken"></param>
        /// <exception cref="MissingAccessTokenException"></exception>
        protected async Task SetAuthoriziationHeader()
        {
            //	access token validation
            if (string.IsNullOrWhiteSpace(this._accessToken)) await Reauthorize();

            //  clear access token
            this._httpClient.DefaultRequestHeaders.Remove("Authorization");

            //	add bearer token to header
            this._httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {this._accessToken}");
        }

        /// <summary>
        /// When http request failed because of the status is unauthorized
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="response"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        protected async Task HttpRequestUnauthorizedHandler<TaskType>(Func<Task<TaskType>> callback)
        {
            await Reauthorize();
            await callback();
        }
    }
}

