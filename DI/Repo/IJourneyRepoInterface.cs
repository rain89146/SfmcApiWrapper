using System;
using SalesforceMarketingCloudIntegration.Models;

namespace SalesforceMarketingCloudIntegration.DI
{
	public interface IJourneyRepoInterface
	{
        Task<JourneyEventResponse> FireJourneyEvent(string contactKey, string eventDefinitionKey, Dictionary<string, string> eventData);
    }
}

