using System;
using System.Threading.Tasks;

namespace Sarjee.SimpleRenamer.Common.Interface
{
    /// <summary>
    /// BackgroundQueue Interface
    /// </summary>
    public interface IBackgroundQueue
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        Task QueueTaskAsync(Action action);

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="work"></param>
        /// <returns></returns>
        Task<T> QueueTaskAsync<T>(Func<T> work);
    }
}
