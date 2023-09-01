using System;
namespace SalesforceMarketingCloudIntegration.Exceptions
{
	public class ForbiddenAccessException: Exception
	{
		//
		private readonly string _message;

		//
		public ForbiddenAccessException(string message) => this._message = message;

		//
        public override string Message => this._message;
    }
}

