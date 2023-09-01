using System;
namespace SalesforceMarketingCloudIntegration.Exceptions
{
	public class UnableToInsertRowIntoDataExtension: Exception
	{
		//
		private readonly string _message;

		//
		public UnableToInsertRowIntoDataExtension(string message) => this._message = $"Failed to insert row into data extension. reason: {message}";

		//
		public override string Message => this._message;
		
	}
}

