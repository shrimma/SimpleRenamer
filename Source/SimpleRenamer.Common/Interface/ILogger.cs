using System;
using System.Diagnostics.Tracing;

namespace Sarjee.SimpleRenamer.Common.Interface
{
    /// <summary>
    /// Logger Interface
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Logs a message
        /// </summary>
        /// <param name="message">The message to log</param>
        /// <param name="logType">Type of the log.</param>
        /// <param name="memberName">Name of the member.</param>
        /// <param name="sourceFilePath">The source file path.</param>
        /// <param name="sourceLineNumber">The source line number.</param>
        void TraceMessage(string message = "", EventLevel logType = EventLevel.Informational,
        [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
        [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
        [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0);

        /// <summary>
        /// Logs an exception
        /// </summary>
        /// <param name="ex">The exception to log</param>
        /// <param name="message">The message to log</param>
        /// <param name="memberName">Name of the member.</param>
        /// <param name="sourceFilePath">The source file path.</param>
        /// <param name="sourceLineNumber">The source line number.</param>
        void TraceException(Exception ex, string message = "",
            [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
        [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
        [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0);
    }
}
