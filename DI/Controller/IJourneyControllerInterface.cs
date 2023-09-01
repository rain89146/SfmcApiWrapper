using System;
namespace SalesforceMarketingCloudIntegration.DI.Controller
{
	public interface IJourneyControllerInterface
	{
		Task<string> FireJourneyEvent(string contactKey, string eventDefinitionKey, Dictionary<string, string> eventData);
	}
}

