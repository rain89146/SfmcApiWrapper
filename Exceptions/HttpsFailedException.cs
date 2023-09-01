using System;
using System.Text;

namespace SalesforceMarketingCloudIntegration.Exceptions
{
	public class HttpsFailedException: Exception
	{
		//
		private readonly string _message;

		//
        public HttpsFailedException(string? message = null, int? statusCode = null)
		{
            StringBuilder sb = new ("Http failed to make request.");

			//
			if (string.IsNullOrWhiteSpace(message)) sb.Append($" Reason: {message}");

			//
			if (statusCode is not null) sb.Append($" Code: {statusCode}");

			//
            this._message = sb.ToString();
		}

		//
		public override string Message => this._message;
    }
}

