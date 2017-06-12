using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sarjee.SimpleRenamer.Common.Extensions
{
    /// <summary>
    /// TaskExtensions
    /// </summary>
    public static class TaskExtensions
    {
        /// <summary>
        /// Ins the completion order.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static IEnumerable<Task<T>> InCompletionOrder<T>(this IEnumerable<Task<T>> source)
        {
            var inputs = source.ToList();
            var boxes = inputs.Select(x => new TaskCompletionSource<T>()).ToList();

            int currentIndex = -1;
            foreach (var task in inputs)
            {
                task.ContinueWith(completed =>
                {
                    var nextBox = boxes[Interlocked.Increment(ref currentIndex)];
                    PropagateResult(completed, nextBox);
                }, TaskContinuationOptions.ExecuteSynchronously);
            }
            return boxes.Select(box => box.Task);
        }

        /// <summary>
        /// Propagates the status of the given task (which must be completed) to a task completion source
        /// (which should not be).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="completedTask">The completed task.</param>
        /// <param name="completionSource">The completion source.</param>
        /// <exception cref="System.ArgumentException">Task was not completed</exception>
        private static void PropagateResult<T>(Task<T> completedTask,
            TaskCompletionSource<T> completionSource)
        {
            switch (completedTask.Status)
            {
                case TaskStatus.Canceled:
                    completionSource.TrySetCanceled();
                    break;
                case TaskStatus.Faulted:
                    completionSource.TrySetException(completedTask.Exception.InnerExceptions);
                    break;
                case TaskStatus.RanToCompletion:
                    completionSource.TrySetResult(completedTask.Result);
                    break;
                default:
                    throw new ArgumentException("Task was not completed");
            }
        }
    }
}
