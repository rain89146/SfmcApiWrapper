using System;
namespace SalesforceMarketingCloudIntegration.Exceptions
{
	public class UnableToProcessDataExtensionRequest : Exception
	{
		private readonly string _message;

		public UnableToProcessDataExtensionRequest(string message)
		{
			this._message = message;
		}

		public override string Message
		{
			get
			{
				return $"Failed to insert row into data extension. reason: {this._message}";
			}
		}
	}
}

