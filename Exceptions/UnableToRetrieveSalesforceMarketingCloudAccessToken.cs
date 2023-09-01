using System;
namespace SalesforceMarketingCloudIntegration.Exceptions
{
	public class UnableToRetrieveSalesforceMarketingCloudAccessToken: Exception
	{
        public override string Message
		{
			get
			{
				return "Unable to retrieve salesforce marketing cloud access token";
			}
		}
    }
}

