using OneTrueError.Client;
using Sarjee.SimpleRenamer.Common.Interface;
using Sarjee.SimpleRenamer.Common.Model;
using System;
namespace Sarjee.SimpleRenamer.Logging
{
    public class Logger : ILogger
    {
        private log4net.ILog log { get; set; }

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

        private static void IgnoreBadCertificate()
        {
            System.Net.ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(AcceptAllCertifications);
        }

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

        public void TraceMessage(string message = "", LogType logType = LogType.Info,
        [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
        [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
        [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
        {
            //lets always trace messages
            System.Diagnostics.Trace.WriteLine("message: " + message);
            System.Diagnostics.Trace.WriteLine("member name: " + memberName);
            System.Diagnostics.Trace.WriteLine("source file path: " + sourceFilePath);
            System.Diagnostics.Trace.WriteLine("source line number: " + sourceLineNumber);

            switch (logType)
            {
                case LogType.Info:
                    log.Info(message);
                    break;
                case LogType.Warning:
                    log.Warn(string.Format("Warning: {0}, Member Name {1}, Source File {2}, Source Line {3}", message, memberName, sourceFilePath, sourceLineNumber));
                    break;
                case LogType.Error:
                    log.Error(string.Format("Error: {0}, Member Name {1}, Source File {2}, Source Line {3}", message, memberName, sourceFilePath, sourceLineNumber));
                    break;
            }
        }

        public void TraceException(Exception ex, string message = "",
            [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
        [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
        [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
        {
            //lets always trace messages
            System.Diagnostics.Trace.WriteLine("message: " + message);
            System.Diagnostics.Trace.WriteLine("member name: " + memberName);
            System.Diagnostics.Trace.WriteLine("source file path: " + sourceFilePath);
            System.Diagnostics.Trace.WriteLine("source line number: " + sourceLineNumber);
            System.Diagnostics.Trace.WriteLine("exception: " + ex.ToString());
            System.Diagnostics.Trace.WriteLine("exception message: " + ex.Message);
            if (ex.InnerException != null)
            {
                System.Diagnostics.Trace.WriteLine("inner exception message: " + ex.InnerException.Message);
            }

            string innerEx = ex.InnerException == null ? "" : $", InnerException: {ex.InnerException.Message}";
            string logthis = string.Format("Message: {0}, Caller Member: {1}, Source File Path: {2}, Source Line Number: {3}, Exception: {4}, Message: {5}{6}", message, memberName, sourceFilePath, sourceLineNumber.ToString(), ex.ToString(), ex.Message, innerEx);
            log.Fatal(logthis, ex);
        }
    }
}

