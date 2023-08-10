using System;
namespace SalesforceMarketingCloudIntegration
{
	public class HttpsFailedException: Exception
	{
		private readonly string _message;
		private readonly int _statusCode;


        public HttpsFailedException(string message, int statusCode)
		{
			this._message = message;
			this._statusCode = statusCode;
		}

        public override string Message
		{
			get
			{
				return $"Http failed to make request. Reason: {this._message}. Code: {this._statusCode}";
			}
		}
    }
}

