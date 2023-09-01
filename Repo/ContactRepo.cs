using System;
using Newtonsoft.Json;
using SalesforceMarketingCloudIntegration.Exceptions;
using System.Net;
using System.Text;
using SalesforceMarketingCloudIntegration.Models;
using SalesforceMarketingCloudIntegration.DI;

namespace SalesforceMarketingCloudIntegration.Repo
{
	public class ContactRepo: CloudBase, IContactRepoInterface
    {
		public ContactRepo(HttpClient client, MarketingCloudClientConfigObject credential) : base(client, credential) { }

        /// <summary>
        /// Validate email address
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <returns></returns>
        /// <exception cref="HttpsFailedException"></exception>
        /// <exception cref="ForbiddenAccessException"></exception>
        public async Task<bool> ValidateEmailAddress(string emailAddress)
        {
            //  add bearer token to header
            await SetAuthoriziationHeader();

            List<string> validator = new() { "SyntaxValidator", "MXValidator", "ListDetectiveValidator" };

            //	compose payload string
            string json = JsonConvert.SerializeObject(
                new
                {
                    email = emailAddress,
                    validators = validator
                }
            );
            StringContent httpContent = new(json, Encoding.UTF8, "application/json");

            //
            var requestUrl = new Uri($"{this._restUrl}address/v1/validateEmail");

            //
            using HttpResponseMessage response = await this._httpClient.PostAsync(requestUrl, httpContent).ConfigureAwait(false);

            //
            string responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            //
            if (response.IsSuccessStatusCode == false)
            {
                //
                if (response is null || response.ReasonPhrase is null) throw new HttpsFailedException();

                //
                if (response.StatusCode == HttpStatusCode.Forbidden) throw new ForbiddenAccessException(response.ReasonPhrase);

                //
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    await Reauthorize();
                    await ValidateEmailAddress(emailAddress);
                }

                //
                throw new HttpsFailedException(response.ReasonPhrase, (int)response.StatusCode);
            }

            Console.WriteLine(response.IsSuccessStatusCode);
            Console.WriteLine(response.StatusCode == HttpStatusCode.Forbidden);
            Console.WriteLine((int)response.StatusCode);
            Console.WriteLine(response.ReasonPhrase);
            Console.WriteLine(responseContent);

            return true;
        }
    }
}

