using System;
namespace SalesforceMarketingCloudIntegration.Exceptions
{
	public class UnableToProcessDataExtensionRequest : Exception
	{
		//
		private readonly string _message;

		//
		public UnableToProcessDataExtensionRequest(string message) => this._message = $"Failed to insert row into data extension. reason: {message}";

		//
		public override string Message => this._message;
	}
}

