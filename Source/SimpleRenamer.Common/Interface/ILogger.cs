using Sarjee.SimpleRenamer.Common.Model;
using System;

namespace Sarjee.SimpleRenamer.Common.Interface
{
    public interface ILogger
    {
        /// <summary>
        /// Logs a message
        /// </summary>
        /// <param name="message">The message to log</param>
        /// <param name="logType">The logtype for the log entry</param>
        void TraceMessage(string message = "", LogType logType = LogType.Info,
        [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
        [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
        [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0);

        /// <summary>
        /// Logs an exception
        /// </summary>
        /// <param name="ex">The exception to log</param>
        /// <param name="message">The message to log</param>
        void TraceException(Exception ex, string message = "",
            [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
        [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
        [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0);
    }
}
