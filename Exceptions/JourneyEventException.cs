using System;
namespace SalesforceMarketingCloudIntegration.Exceptions
{
	public class JourneyEventException: Exception
	{
		//
		private readonly string _message;

		//
		public JourneyEventException(string message) => this._message = $"Journey failed to start. Reason: {message}";

		//
        public override string Message => this._message;
    }
}

