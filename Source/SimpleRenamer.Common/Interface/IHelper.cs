using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sarjee.SimpleRenamer.Common.Interface
{
    /// <summary>
    /// Helper interface
    /// </summary>
    public interface IHelper
    {
        /// <summary>
        /// Checks whether the input is a valid file extension
        /// </summary>
        /// <param name="fExt">The file extension to process</param>
        /// <returns>True if a valid file extension</returns>
        bool IsFileExtensionValid(string fExt);

        /// <summary>
        /// Checks if the input lists are equal.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listOne">The list one.</param>
        /// <param name="listTwo">The list two.</param>
        /// <returns></returns>
        bool AreListsEqual<T>(List<T> listOne, List<T> listTwo);

        /// <summary>
        /// Removes any special characters.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        string RemoveSpecialCharacters(string input);

        /// <summary>
        /// Adds asynchronous exponential delay
        /// </summary>
        /// <param name="offsetMilliseconds">The offset milliseconds.</param>
        /// <param name="retryCount">The retry count.</param>
        /// <param name="maxBackoffSeconds">The maximum backoff seconds.</param>
        /// <returns></returns>
        Task ExponentialDelayAsync(int offsetMilliseconds, int retryCount, int maxBackoffSeconds, CancellationToken cancellationToken);

        /// <summary>
        /// Executes the rest request.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="restClient">The rest client.</param>
        /// <param name="restRequest">The rest request.</param>
        /// <param name="jsonSerializerSettings">The json serializer settings.</param>
        /// <param name="maxRetryCount">The maximum retry count.</param>
        /// <param name="maxBackoffSeconds">The maximum backoff seconds.</param>
        /// <param name="loginCallback">The login callback.</param>
        /// <returns></returns>
        Task<T> ExecuteRestRequestAsync<T>(IRestClient restClient, IRestRequest restRequest, JsonSerializerSettings jsonSerializerSettings, int maxRetryCount, int maxBackoffSeconds, CancellationToken cancellationToken, Func<Task> loginCallback = null) where T : class;
    }
}
