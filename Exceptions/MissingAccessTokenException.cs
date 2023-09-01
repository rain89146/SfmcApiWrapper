using System;
namespace SalesforceMarketingCloudIntegration.Exceptions
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

