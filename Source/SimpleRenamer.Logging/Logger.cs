﻿using Sarjee.SimpleRenamer.Common.Interface;
using Serilog;
using System;
using System.Diagnostics.Tracing;

namespace Sarjee.SimpleRenamer.Logging
{
    /// <summary>
    /// Logger
    /// </summary>
    /// <seealso cref="Sarjee.SimpleRenamer.Common.Interface.ILogger" />
    public class Logger : Common.Interface.ILogger
    {
        private Serilog.ILogger _logger { get; set; }
        private const string _defaultTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}";
        private const string _messageTemplate = "{Message} - MemberName: {MemberName}, SourceFile: {SourceFilePath}, Source Line: {SourceLineNumber}.";

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

            //serilog configuration
            LoggerConfiguration loggerConfiguration = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.RollingFile(@"logs\log-{Date}.txt", outputTemplate: _defaultTemplate)
                .WriteTo.Trace();

            _logger = loggerConfiguration.CreateLogger();

            //TODO reenable OTE in the future
            ////TODO FACTOR THIS OUT WITH A HTTPS CERT
            ////ignore the certificate issue with OTE server
            //IgnoreBadCertificate();

            ////OTE config
            //OneTrue.Configuration.Credentials(new Uri(configManager.OneTrueErrorUrl), configManager.OneTrueErrorApplicationKey, configManager.OneTrueErrorSharedSecret);
            //OneTrue.Configuration.CatchLog4NetExceptions();
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
            switch (logType)
            {
                case EventLevel.Verbose:
                    _logger.Verbose(_messageTemplate, message, memberName, sourceFilePath, sourceLineNumber);
                    break;
                case EventLevel.LogAlways:
                case EventLevel.Informational:
                    _logger.Information(_messageTemplate, message, memberName, sourceFilePath, sourceLineNumber);
                    break;
                case EventLevel.Warning:
                    _logger.Warning(_messageTemplate, message, memberName, sourceFilePath, sourceLineNumber);
                    break;
                case EventLevel.Error:
                    _logger.Error(_messageTemplate, message, memberName, sourceFilePath, sourceLineNumber);
                    break;
                case EventLevel.Critical:
                    _logger.Fatal(_messageTemplate, message, memberName, sourceFilePath, sourceLineNumber);
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
            _logger.Fatal(ex, _messageTemplate, message, memberName, sourceFilePath, sourceLineNumber);
            if (ex.InnerException != null)
            {
                TraceException(ex.InnerException, "InnerException", memberName, sourceFilePath, sourceLineNumber);
            }
        }
    }
}

