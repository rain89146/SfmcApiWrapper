using System;
namespace SalesforceMarketingCloudIntegration.DI
{
	public interface IContactRepoInterface
	{
        Task<bool> ValidateEmailAddress(string emailAddress);
    }
}

