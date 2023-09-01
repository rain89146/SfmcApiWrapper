using System;
using Newtonsoft.Json;
using SalesforceMarketingCloudIntegration.Exceptions;
using System.Net;
using System.Text;
using SalesforceMarketingCloudIntegration.Models;
using SalesforceMarketingCloudIntegration.DI;

namespace SalesforceMarketingCloudIntegration.Repo
{
	public class JourneyRepo: CloudBase, IJourneyRepoInterface
    {
		public JourneyRepo(HttpClient httpClient, MarketingCloudClientConfigObject credential) : base(httpClient, credential) { }

        /// <summary>
        /// Fire a jorueny event
        /// </summary>
        /// <remarks>Visit https://developer.salesforce.com/docs/marketing/marketing-cloud/guide/postEvent.html for documentation</remarks>
        /// <param name="contactKey"></param>
        /// <param name="eventDefinitionKey"></param>
        /// <param name="eventData"></param>
        /// <returns></returns>
        /// <exception cref="JourneyEventException"></exception>
        /// <exception cref="HttpsFailedException"></exception>
		public async Task<JourneyEventResponse> FireJourneyEvent(string contactKey, string eventDefinitionKey, Dictionary<string, string> eventData)
		{
            //	add bearer token to header
            await SetAuthoriziationHeader();

            //	compose payload string
            string json = JsonConvert.SerializeObject(
                new
                {
                    ContactKey = contactKey,
                    EventDefinitionKey = eventDefinitionKey,
                    Data = eventData
                }
            );
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            //	build request url
            var requestUrl = new Uri($"{this._restUrl}interaction/v1/events");

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
                    await FireJourneyEvent(contactKey, eventDefinitionKey, eventData);
                }

                //	if it's salesforce error
                if (response.StatusCode == HttpStatusCode.BadRequest && response.StatusCode == HttpStatusCode.Forbidden)
                {
                    //  parse the error
                    var parsedError = JsonConvert.DeserializeObject<JourneyEventErrorResponse>(responseContent) ?? throw new JourneyEventException("Empty error response");

                    //
                    throw new JourneyEventException(parsedError.message);
                }

                //
                throw new HttpsFailedException(response.ReasonPhrase, (int)response.StatusCode);
            }

            //  no response string
            if (string.IsNullOrEmpty(responseContent)) throw new JourneyEventException("Missing response content");

            //  parse response content
            var parsedResult = JsonConvert.DeserializeObject<JourneyEventResponse>(responseContent);

            //	parse into object
            return parsedResult ?? throw new JourneyEventException("Can't parse response content");
        }
	}
}

