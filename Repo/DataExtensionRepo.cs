using System.Net;
using System.Text;
using Newtonsoft.Json;
using SalesforceMarketingCloudIntegration.Exceptions;
using SalesforceMarketingCloudIntegration.Models;
using SalesforceMarketingCloudIntegration.DI;

namespace SalesforceMarketingCloudIntegration.Repo
{
	public class DataExtensionRepo: CloudBase, IDataExtensionRepoInterface
    {
        public DataExtensionRepo(HttpClient httpClient, MarketingCloudClientConfigObject credential) : base(httpClient, credential) { }
		
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

            //	Http request failed
            if (!response.IsSuccessStatusCode)
            {
                //  if it's unauthorized, get authorized and try again
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    await Reauthorize();
                    await InsertRowIntoDataExtensionAsync(DataExtensionKeyId, rows);
                }

                //	if it's salesforce error
                if (response.StatusCode == HttpStatusCode.BadRequest) SalesforceDataExtensionErrorResponse(responseContent);

                //
                throw new HttpsFailedException(response.ReasonPhrase, (int)response.StatusCode);
            }

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
            string responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            //	Http request failed
            if (!response.IsSuccessStatusCode)
            {
                //  if it's unauthorized
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    await Reauthorize();
                    await UpsertRowIntoDataExtensionAsync(DataExtensionKeyId, rows);
                }

                //	if it's salesforce error
                if (response.StatusCode == HttpStatusCode.BadRequest) SalesforceDataExtensionErrorResponse(responseContent);

                //  other unknown http issue
                throw new HttpsFailedException(response.ReasonPhrase, (int)response.StatusCode);
            }

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
            using HttpResponseMessage response = await this._httpClient.GetAsync(requestUrl).ConfigureAwait(false);

            //  get response string
            string responseContent = await response.Content.ReadAsStringAsync();

            //  when http request failed
            if (!response.IsSuccessStatusCode)
            {
                //  when nothing being returned
                if (response is null || response.ReasonPhrase is null) throw new HttpsFailedException();

                //  when it's unauthorized
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    await Reauthorize();
                    await RetrieveStatusOfRequest(requestId);
                }

                //  other unknown http issue
                throw new HttpsFailedException(response.ReasonPhrase, (int)response.StatusCode);
            }

            //  response content is null
            if (string.IsNullOrEmpty(responseContent)) throw new UnableRetrieveStatusOfRequestException();

            //  parse response content
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
            string responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            //
            if (!response.IsSuccessStatusCode)
            {
                //  when nothing being returned
                if (response is null || response.ReasonPhrase is null) throw new HttpsFailedException();

                //  when it's unauthorized
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    await Reauthorize();
                    await RetrieveResultOfRequest(requestId);
                }

                //  other unknown http issue
                throw new HttpsFailedException(response.ReasonPhrase, (int)response.StatusCode);
            }

            //  response content is null
            if (string.IsNullOrEmpty(responseContent)) throw new UnableRetrieveStatusOfRequestException();

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
    }
}

