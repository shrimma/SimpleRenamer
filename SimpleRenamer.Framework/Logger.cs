using OneTrueError.Client;
using SimpleRenamer.Framework.DataModel;
using SimpleRenamer.Framework.Interface;
using System;
namespace SimpleRenamer.Framework
{
    public class Logger : SimpleRenamer.Framework.Interface.ILogger
    {
        private log4net.ILog log { get; set; }

        public Logger(IConfigurationManager configManager)
        {
            if (configManager == null)
            {
                throw new ArgumentNullException(nameof(configManager));
            }
            log = log4net.LogManager.GetLogger(typeof(Logger));
            log4net.Config.XmlConfigurator.Configure();

            //ignore the certificate issue with OTE server
            IgnoreBadCertificate();
            OneTrue.Configuration.Credentials(new Uri(configManager.OneTrueErrorUrl), configManager.OneTrueErrorApplicationKey, configManager.OneTrueErrorSharedSecret);
            OneTrue.Configuration.CatchLog4NetExceptions();
        }

        private static void IgnoreBadCertificate()
        {
            System.Net.ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(AcceptAllCertifications);
        }

        private static bool AcceptAllCertifications(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certification, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            //ignore certificate errors for the OTE server
            if (certification.Issuer.Equals("CN=onetrueerror-vm"))
            {
                return true;
            }
            else
            {
                return false;
            }
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

            string innerEx = ex.InnerException == null ? "" : ex.InnerException.Message;
            string logthis = string.Format("Message: {0}, Caller Member: {1}, Source File Path: {2}, Source Line Number: {3}, Exception: {4}, Message: {5}, Inner Exception: {6}", message, memberName, sourceFilePath, sourceLineNumber.ToString(), ex.ToString(), ex.Message, innerEx);
            log.Fatal(logthis, ex);
        }
    }
}

