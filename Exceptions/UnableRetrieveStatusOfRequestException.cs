using System;
namespace SalesforceMarketingCloudIntegration.Exceptions
{
	public class UnableRetrieveStatusOfRequestException: Exception
	{
		//
		private readonly string _message;

		//
		public UnableRetrieveStatusOfRequestException() => this._message = "Unabele to retrieve request status";

        //
        public override string Message => this._message;
    }
}

