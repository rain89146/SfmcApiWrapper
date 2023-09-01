using System;
namespace SalesforceMarketingCloudIntegration.Helper
{
	public static class Tools
	{
        /// <summary>
        /// Do retry async
        /// </summary>
        /// <typeparam name="ReturnType"></typeparam>
        /// <param name="callback"></param>
        /// <param name="retryInterval"></param>
        /// <param name="maxRetries"></param>
        /// <returns></returns>
        /// <exception cref="AggregateException"></exception>
        public static async Task<ReturnType> DoRetryAsync<ReturnType>(Func<Task<ReturnType>> callback, TimeSpan retryInterval, int maxRetries)
        {
            //
            List<Exception> exceptions = new();

            //  counter for times of retry
            int retryCounter = 0;

            //
            while (retryCounter <= maxRetries)
            {
                try
                {
                    //  wait for certain time between each retry
                    if (retryCounter > 0) await Task.Delay(retryInterval);

                    //  returns request id
                    return await callback();
                }
                catch (Exception ex)
                {
                    //  add to counter
                    retryCounter++;

                    //  append exception
                    exceptions.Add(ex);
                }
            }

            //  throw all combine exception
            throw new AggregateException(exceptions);
        }
    }
}

