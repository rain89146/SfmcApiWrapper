using System;
using SalesforceMarketingCloudIntegration.DI;
using SalesforceMarketingCloudIntegration.DI.Controller;
using SalesforceMarketingCloudIntegration.Helper;

namespace SalesforceMarketingCloudIntegration.Controller
{
	public class ContactController: IContactControllerInterface
    {
        //
        private readonly IContactRepoInterface _repo;

        //
        private readonly int maxRetries = 5;
        private readonly int retriesOffsetTimeMS = 500;

        //
        public ContactController(IContactRepoInterface repo) => this._repo = repo;

        //
        public async Task<bool> ValidateEmailAddress(string emailAddress)
        {
            var response = await Tools.DoRetryAsync(
                async () => await _repo.ValidateEmailAddress(emailAddress),
                TimeSpan.FromMilliseconds(retriesOffsetTimeMS),
                maxRetries
                );

            return response;
        }
    }
}

