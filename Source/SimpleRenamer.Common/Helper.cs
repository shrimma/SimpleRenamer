using Sarjee.SimpleRenamer.Common.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Sarjee.SimpleRenamer.Common
{
    /// <summary>
    /// Helper
    /// </summary>
    /// <seealso cref="Sarjee.SimpleRenamer.Common.Interface.IHelper" />
    public class Helper : IHelper
    {
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
        public async Task ExponentialDelayAsync(int offsetMilliseconds, int retryCount, int maxBackoffSeconds)
        {
            //generate a TimeSpan to use for backoff based on offset * 2^retries
            TimeSpan backoff = TimeSpan.FromMilliseconds(offsetMilliseconds * (int)Math.Pow(2, retryCount));

            //if the back off in seconds exceeds our max backoff then set the backoff to maxbackoff
            if (backoff.Seconds > maxBackoffSeconds)
            {
                backoff = TimeSpan.FromSeconds(maxBackoffSeconds);
            }

            //await the backoff delay
            await Task.Delay(backoff);
        }

        //private int[] httpStatusCodesWorthRetrying = { 408, 500, 502, 503, 504, 598, 599 };
        //public async Task<T> ExecuteRestRequest<T>(IRestClient restClient, IRestRequest restRequest, JsonSerializerSettings jsonSerializerSettings, int maxRetryCount, int maxBackoffSeconds, Func<Task> LoginCallback) where T : class
        //{
        //    int currentRetry = 0;
        //    int offset = ThreadLocalRandom.Instance.Next(100, 500);
        //    while (currentRetry < maxRetryCount)
        //    {
        //        try
        //        {
        //            //execute the request
        //            IRestResponse response = await restClient.ExecuteTaskAsync(restRequest);
        //            //if no errors and statuscode ok then deserialize the response
        //            if (response.ErrorException == null && response?.StatusCode == HttpStatusCode.OK)
        //            {
        //                T result = JsonConvert.DeserializeObject<T>(response.Content, jsonSerializerSettings);
        //                return result;
        //            }
        //            //if status code indicates transient error then throw timeoutexception
        //            else if (httpStatusCodesWorthRetrying.Contains((int)response?.StatusCode))
        //            {
        //                throw new TimeoutException();
        //            }
        //            //if status code indicates unauthorized then throw unauthorized exception
        //            else if (response?.StatusCode == HttpStatusCode.Unauthorized)
        //            {
        //                throw new UnauthorizedAccessException();
        //            }
        //            //else throw the responses exception
        //            else
        //            {
        //                throw response.ErrorException;
        //            }
        //        }
        //        catch (TimeoutException)
        //        {
        //            currentRetry++;
        //            await ExponentialDelayAsync(offset, currentRetry, maxBackoffSeconds);
        //        }
        //        catch (WebException)
        //        {
        //            currentRetry++;
        //            await ExponentialDelayAsync(offset, currentRetry, maxBackoffSeconds);
        //        }
        //        catch (UnauthorizedAccessException)
        //        {
        //            currentRetry++;
        //            if (LoginCallback != null)
        //            {
        //                await LoginCallback();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //    }
        //    return null;
        //}
    }
}
