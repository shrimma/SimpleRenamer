using System;
using System.Threading.Tasks;

namespace SimpleRenamer.Framework.Interface
{
    public interface IBackgroundQueue
    {
        Task QueueTask(Action action);
        Task<T> QueueTask<T>(Func<T> work);
    }
}
