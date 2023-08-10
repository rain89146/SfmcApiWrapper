using System;
namespace SalesforceMarketingCloudIntegration
{
	public class UnableToInsertRowIntoDataExtension: Exception
	{
		private readonly string _message;

		public UnableToInsertRowIntoDataExtension(string message)
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

