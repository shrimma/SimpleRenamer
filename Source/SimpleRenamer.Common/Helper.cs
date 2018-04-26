using Newtonsoft.Json;
using Polly;
using Polly.Retry;
using RestSharp;
using Sarjee.SimpleRenamer.Common.Helpers;
using Sarjee.SimpleRenamer.Common.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Sarjee.SimpleRenamer.Common
{
    /// <summary>
    /// Helper
    /// </summary>
    /// <seealso cref="Sarjee.SimpleRenamer.Common.Interface.IHelper" />
    public class Helper : IHelper
    {
        private static readonly int[] httpStatusCodesWorthRetrying = { 401, 408, 500, 502, 503, 504, 598, 599 };
        private const int maxRetryCount = 10;
        private readonly PolicyBuilder<IRestResponse> _retryPolicyBase = Policy
                .Handle<WebException>()
                .OrResult<IRestResponse>(r => httpStatusCodesWorthRetrying.Contains((int)r.StatusCode));

        /// <summary>
        /// Checks whether the input is a valid file extension
        /// </summary>
        /// <param name="fExt">The file extension to process</param>
        /// <returns>
        /// True if a valid file extension
        /// </returns>
        public bool IsFileExtensionValid(string fExt)
        {
            bool answer = true;
            if (!String.IsNullOrWhiteSpace(fExt) && fExt.Length > 1 && fExt[0] == '.')
            {
                char[] invalidFileChars = Path.GetInvalidFileNameChars();
                foreach (char c in invalidFileChars)
                {
                    if (fExt.Contains(c.ToString()))
                    {
                        answer = false;
                        break;
                    }
                }
            }
            else
            {
                answer = false;
            }
            return answer;
        }

        /// <summary>
        /// Checks if the input lists are equal.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listOne">The list one.</param>
        /// <param name="listTwo">The list two.</param>
        /// <returns></returns>
        public bool AreListsEqual<T>(List<T> listOne, List<T> listTwo)
        {
            if (listOne.Count != listTwo.Count)
            {
                return false;
            }
            if (listOne.Except(listTwo).Any())
            {
                return false;
            }
            if (listTwo.Except(listOne).Any())
            {
                return false;
            }

            return true;
        }

        private static string regexSearch = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
        private Regex specialCharacterRegex = new Regex(string.Format("[{0}]", Regex.Escape(regexSearch)));
        /// <summary>
        /// Removes the special characters.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        public string RemoveSpecialCharacters(string input)
        {
            return specialCharacterRegex.Replace(input, "");
        }

        /// <summary>
        /// Adds asynchronous exponential delay
        /// </summary>
        /// <param name="offsetMilliseconds">The offset milliseconds.</param>
        /// <param name="retryCount">The retry count.</param>
        /// <param name="maxBackoffSeconds">The maximum backoff seconds.</param>
        /// <returns></returns>
        public async Task ExponentialDelayAsync(int offsetMilliseconds, int retryCount, int maxBackoffSeconds, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            //generate a TimeSpan to use for backoff based on offset * 2^retries
            TimeSpan backoff = TimeSpan.FromMilliseconds(offsetMilliseconds * (int)Math.Pow(2, retryCount));

            //if the back off in seconds exceeds our max backoff then set the backoff to maxbackoff
            if (backoff.Seconds > maxBackoffSeconds)
            {
                backoff = TimeSpan.FromSeconds(maxBackoffSeconds);
            }

            //await the backoff delay
            await Task.Delay(backoff, cancellationToken);
        }

        /// <summary>
        /// Executes the request asynchronous.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        /// <remarks>virtual method for testability</remarks>
        protected virtual async Task<IRestResponse> ExecuteRequestAsync(IRestClient restClient, IRestRequest request, CancellationToken cancellationToken)
        {
            return await restClient.ExecuteTaskAsync(request, cancellationToken);
        }

        /// <summary>
        /// Executes a rest request.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="restClient">The rest client.</param>
        /// <param name="restRequest">The rest request.</param>
        /// <param name="jsonSerializerSettings">The json serializer settings.</param>
        /// <param name="maxRetryCount">The maximum retry count.</param>
        /// <param name="maxBackoffSeconds">The maximum backoff seconds.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="loginCallback">The login callback.</param>
        /// <returns></returns>
        /// <exception cref="TimeoutException"></exception>
        /// <exception cref="UnauthorizedAccessException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<T> ExecuteRestRequestAsync<T>(IRestClient restClient, IRestRequest restRequest, JsonSerializerSettings jsonSerializerSettings, int maxRetryCount, int maxBackoffSeconds, CancellationToken cancellationToken, Func<Task> loginCallback = null) where T : class
        {
            //configure retry policy based on the max retry count and if there is a login
            RetryPolicy<IRestResponse> retryPolicy = _retryPolicyBase.WaitAndRetryAsync(maxRetryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), async (response, timespan, retryCount, context) =>
            {
                //log the retry attempt etc verbose
                if (response?.Result?.StatusCode == HttpStatusCode.Unauthorized && loginCallback != null)
                {
                    await loginCallback();
                }
            });

            var actualResponse = await retryPolicy.ExecuteAsync((ct) => ExecuteRequestAsync(restClient, restRequest, ct), cancellationToken);

            if (actualResponse.ErrorException == null && actualResponse?.StatusCode == HttpStatusCode.OK)
            {
                T result = JsonConvert.DeserializeObject<T>(actualResponse.Content, jsonSerializerSettings);
                return result;
            }
            else
            {
                if (actualResponse?.ErrorException != null)
                {
                    throw actualResponse?.ErrorException;
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }
        }
    }
}
