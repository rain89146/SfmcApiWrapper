using System;
using SalesforceMarketingCloudIntegration.DI;
using SalesforceMarketingCloudIntegration.DI.Controller;
using SalesforceMarketingCloudIntegration.Helper;

namespace SalesforceMarketingCloudIntegration.Controller
{
	public class JourneyController: IJourneyControllerInterface
    {
        //
        private readonly IJourneyRepoInterface _repo;

        //
        private readonly int maxRetries = 5;
        private readonly int retriesOffsetTimeMS = 500;

        //
        public JourneyController(IJourneyRepoInterface repo) => this._repo = repo;
        
        //
        public async Task<string> FireJourneyEvent(string contactKey, string eventDefinitionKey, Dictionary<string, string> eventData)
        {
            var response = await Tools.DoRetryAsync(
                async () => await _repo.FireJourneyEvent(contactKey, eventDefinitionKey, eventData),
                TimeSpan.FromMilliseconds(retriesOffsetTimeMS),
                maxRetries
                );

            return response.eventInstanceId;
        }
    }
}

