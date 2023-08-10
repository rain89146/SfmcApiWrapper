using System;
namespace SalesforceMarketingCloudIntegration
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

