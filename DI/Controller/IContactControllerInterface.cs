using System;
namespace SalesforceMarketingCloudIntegration.DI.Controller
{
	public interface IContactControllerInterface
	{
        Task<bool> ValidateEmailAddress(string emailAddress);
    }
}

