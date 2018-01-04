using System.Threading.Tasks;

namespace Sarjee.SimpleRenamer.Common.Interface
{
    /// <summary>
    /// Message Sender Interface
    /// </summary>
    public interface IMessageSender
    {
        /// <summary>
        /// Sends the Json Payload.
        /// </summary>
        /// <param name="jsonPayload">The json payload.</param>
        /// <returns></returns>
        Task SendAsync(string jsonPayload);
    }
}
