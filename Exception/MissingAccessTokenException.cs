using System;
namespace SalesforceMarketingCloudIntegration
{
	public class MissingAccessTokenException: Exception
	{
		public override string Message
		{
			get
			{
				return "Access token is missing";
			}
		}
	}
}

