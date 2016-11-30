using System;
using System.Threading.Tasks;

namespace SimpleRenamer.Common.Interface
{
    public interface IRetryHelper
    {
        /// <summary>
        /// Wrapper for the generic method for async operations that don't return a value
        /// </summary>
        /// <param name="asyncOperation"></param>
        /// <returns></returns>
        Task OperationWithBasicRetryAsync(Func<Task> asyncOperation);
        /// <summary>
        /// Main generic method to perform the supplied async method with multiple retires on transient exceptions/errors
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="asyncOperation"></param>
        /// <returns></returns>
        Task<T> OperationWithBasicRetryAsync<T>(Func<Task<T>> asyncOperation);
    }
}
