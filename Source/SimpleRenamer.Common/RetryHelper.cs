using Sarjee.SimpleRenamer.Common.Interface;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Sarjee.SimpleRenamer.Common
{
    /// <summary>
    /// Retry Helper
    /// </summary>
    /// <seealso cref="Sarjee.SimpleRenamer.Common.Interface.IRetryHelper" />
    public class RetryHelper : IRetryHelper
    {
        private int RETRY_COUNT;
        public RetryHelper(int retryCount = 2)
        {
            RETRY_COUNT = retryCount;
        }

        /// <summary>
        /// Wrapper for the generic method for async operations that don't return a value
        /// </summary>
        /// <param name="asyncOperation"></param>
        /// <returns></returns>
        public async Task OperationWithBasicRetryAsync(Func<Task> asyncOperation)
        {
            await OperationWithBasicRetryAsync<object>(async () =>
            {
                await asyncOperation();
                return null;
            });
        }

        /// <summary>
        /// Main generic method to perform the supplied async method with multiple retires on transient exceptions/errors
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="asyncOperation"></param>
        /// <returns></returns>
        public async Task<T> OperationWithBasicRetryAsync<T>(Func<Task<T>> asyncOperation)
        {
            int currentRetry = 0;

            while (true)
            {
                try
                {
                    return await asyncOperation();
                }
                catch (Exception ex)
                {
                    currentRetry++;

                    if (currentRetry > RETRY_COUNT || !IsTransient(ex))
                    {
                        // If this is not a transient error or we should not retry re-throw the exception.
                        throw;
                    }
                }

                // Wait to retry the operation.
                await Task.Delay(100 * currentRetry);
            }
        }

        /// <summary>
        /// Checks if the provided exception is considered transient in nature or not
        /// Transient include issues such as a single failed network attempt
        /// </summary>
        /// <param name="originalException"></param>
        /// <returns></returns>
        private static bool IsTransient(Exception originalException)
        {
            // If the exception is an HTTP request exception then assume it is transient            
            if (originalException is HttpRequestException)
            {
                return true;
            }

            return false;
        }
    }
}