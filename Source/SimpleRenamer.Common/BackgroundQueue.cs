using Sarjee.SimpleRenamer.Common.Interface;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sarjee.SimpleRenamer.Common
{
    /// <summary>
    /// BackgroundQueue
    /// </summary>
    /// <seealso cref="Sarjee.SimpleRenamer.Common.Interface.IBackgroundQueue" />
    public class BackgroundQueue : IBackgroundQueue
    {
        private Task previousTask = Task.FromResult(true);
        private object key = new object();
        /// <summary>
        /// Queue Task
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public Task QueueTaskAsync(Action action)
        {
            lock (key)
            {
                previousTask = previousTask.ContinueWith(t => action()
                    , CancellationToken.None
                    , TaskContinuationOptions.None
                    , TaskScheduler.Default);
                return previousTask;
            }
        }

        /// <summary>
        /// Queue Task
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="work"></param>
        /// <returns></returns>
        public Task<T> QueueTaskAsync<T>(Func<T> work)
        {
            lock (key)
            {
                var task = previousTask.ContinueWith(t => work()
                    , CancellationToken.None
                    , TaskContinuationOptions.None
                    , TaskScheduler.Default);
                previousTask = task;
                return task;
            }
        }
    }
}
