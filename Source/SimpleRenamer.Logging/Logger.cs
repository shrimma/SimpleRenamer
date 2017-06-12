using OneTrueError.Client;
using Sarjee.SimpleRenamer.Common.Interface;
using System;
using System.Diagnostics.Tracing;

namespace Sarjee.SimpleRenamer.Logging
{
    /// <summary>
    /// Logger
    /// </summary>
    /// <seealso cref="Sarjee.SimpleRenamer.Common.Interface.ILogger" />
    public class Logger : ILogger
    {
        private log4net.ILog log { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Logger"/> class.
        /// </summary>
        /// <param name="configManager">The configuration manager.</param>
        /// <exception cref="System.ArgumentNullException">
        /// configManager
        /// or
        /// OneTrueErrorUrl
        /// or
        /// OneTrueErrorApplicationKey
        /// or
        /// OneTrueErrorSharedSecret
        /// </exception>
        public Logger(IConfigurationManager configManager)
        {
            if (configManager == null)
            {
                throw new ArgumentNullException(nameof(configManager));
            }
            if (string.IsNullOrWhiteSpace(configManager.OneTrueErrorUrl))
            {
                throw new ArgumentNullException(nameof(configManager.OneTrueErrorUrl));
            }
            if (string.IsNullOrWhiteSpace(configManager.OneTrueErrorApplicationKey))
            {
                throw new ArgumentNullException(nameof(configManager.OneTrueErrorApplicationKey));
            }
            if (string.IsNullOrWhiteSpace(configManager.OneTrueErrorSharedSecret))
            {
                throw new ArgumentNullException(nameof(configManager.OneTrueErrorSharedSecret));
            }

            //log4net config
            log = log4net.LogManager.GetLogger(typeof(Logger));
            log4net.Config.XmlConfigurator.Configure();

            //TODO FACTOR THIS OUT WITH A HTTPS CERT
            //ignore the certificate issue with OTE server
            IgnoreBadCertificate();

            //OTE config
            OneTrue.Configuration.Credentials(new Uri(configManager.OneTrueErrorUrl), configManager.OneTrueErrorApplicationKey, configManager.OneTrueErrorSharedSecret);
            OneTrue.Configuration.CatchLog4NetExceptions();
        }

        /// <summary>
        /// Ignores the bad certificate.
        /// </summary>
        private static void IgnoreBadCertificate()
        {
            System.Net.ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(AcceptAllCertifications);
        }

        /// <summary>
        /// Accepts all certifications.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="certification">The certification.</param>
        /// <param name="chain">The chain.</param>
        /// <param name="sslPolicyErrors">The SSL policy errors.</param>
        /// <returns></returns>
        private static bool AcceptAllCertifications(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certification, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            ////ignore certificate errors for the OTE server
            //if (certification.Issuer.Equals("CN=onetrueerror-vm"))
            //{
            //    return true;
            //}
            //else
            //{
            //    return false;
            //}

            return true;
        }

        /// <summary>
        /// Logs a message
        /// </summary>
        /// <param name="message">The message to log</param>
        /// <param name="logType">Type of the log.</param>
        /// <param name="memberName">Name of the member.</param>
        /// <param name="sourceFilePath">The source file path.</param>
        /// <param name="sourceLineNumber">The source line number.</param>
        public void TraceMessage(string message = "", EventLevel logType = EventLevel.Informational,
        [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
        [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
        [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
        {
            //lets always trace messages
            System.Diagnostics.Trace.WriteLine("message: " + message);
            System.Diagnostics.Trace.WriteLine(string.Format("member name: {0}, source file path: {1}, source line number: {2}.", memberName, sourceFilePath, sourceLineNumber.ToString()));

            switch (logType)
            {
                case EventLevel.LogAlways:
                case EventLevel.Verbose:
                case EventLevel.Informational:
                    log.Info(message);
                    break;
                case EventLevel.Warning:
                    log.Warn(string.Format("Warning: {0}, Member Name {1}, Source File {2}, Source Line {3}", message, memberName, sourceFilePath, sourceLineNumber));
                    break;
                case EventLevel.Error:
                    log.Error(string.Format("Error: {0}, Member Name {1}, Source File {2}, Source Line {3}", message, memberName, sourceFilePath, sourceLineNumber));
                    break;
                case EventLevel.Critical:
                    log.Fatal(string.Format("Fatal: {0}, Member Name {1}, Source File {2}, Source Line {3}", message, memberName, sourceFilePath, sourceLineNumber));
                    break;
            }
        }

        /// <summary>
        /// Logs an exception
        /// </summary>
        /// <param name="ex">The exception to log</param>
        /// <param name="message">The message to log</param>
        /// <param name="memberName">Name of the member.</param>
        /// <param name="sourceFilePath">The source file path.</param>
        /// <param name="sourceLineNumber">The source line number.</param>
        public void TraceException(Exception ex, string message = "",
            [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
        [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
        [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
        {
            //lets always trace messages
            System.Diagnostics.Trace.WriteLine("message: " + message);
            System.Diagnostics.Trace.WriteLine(string.Format("member name: {0}, source file path: {1}, source line number: {2}.", memberName, sourceFilePath, sourceLineNumber.ToString()));
            System.Diagnostics.Trace.WriteLine(string.Format("exceptiontype: {0}, message: {1}.", ex.GetType().ToString(), ex.Message));

            if (ex.InnerException != null)
            {
                System.Diagnostics.Trace.WriteLine(string.Format("inner exception type: {0}, message: {1}", ex.InnerException.GetType().ToString(), ex.InnerException.Message));
            }

            string innerEx = ex.InnerException == null ? "" : $", InnerException type: {ex.InnerException.GetType().ToString()}, message: {ex.InnerException.Message}";
            string logthis = string.Format("Message: {0}, Caller Member: {1}, Source File Path: {2}, Source Line Number: {3}, Exception: {4}, Message: {5}{6}", message, memberName, sourceFilePath, sourceLineNumber.ToString(), ex.GetType().ToString(), ex.Message, innerEx);
            log.Fatal(logthis, ex);
        }
    }
}

