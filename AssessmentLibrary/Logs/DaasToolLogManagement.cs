using log4net;
using log4net.Appender;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AssessmentLibrary.Logs
{
    public static class DaasToolLogManagement
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof(DaasToolLogManagement));

        // From http://stackoverflow.com/a/2916628/1067721
        private class LogFileCleanupTask
        {
            #region - Constructor -
            public LogFileCleanupTask()
            {
            }
            #endregion

            #region - Methods -
            /// <summary>
            /// Cleans up. Auto configures the cleanup based on the log4net configuration
            /// </summary>
            /// <param name="date">Anything prior will not be kept.</param>
            public void CleanUp(DateTime date)
            {
                string directory = string.Empty;
                string filePrefix = string.Empty;

                var repo = LogManager.GetAllRepositories().FirstOrDefault();
                if (repo == null)
                    throw new NotSupportedException("Log4Net has not been configured yet.");

                var app = repo.GetAppenders().Where(x => x.GetType() == typeof(RollingFileAppender)).FirstOrDefault();
                if (app != null)
                {
                    var appender = app as RollingFileAppender;

                    directory = Path.GetDirectoryName(appender.File);
                    filePrefix = Path.GetFileName(appender.File);

                    CleanUp(directory, filePrefix, date);
                }
            }
            /// <summary>
            /// Cleans up. Auto configures the cleanup based on the log4net configuration
            /// </summary>
            /// <param name="date">Anything prior will not be kept.</param>
            public void CleanUp(string logPrefix, DateTime date)
            {
                string directory = string.Empty;

                var repo = LogManager.GetAllRepositories().FirstOrDefault();
                if (repo == null)
                    throw new NotSupportedException("Log4Net has not been configured yet.");

                var app = repo.GetAppenders().Where(x => x.GetType() == typeof(RollingFileAppender)).FirstOrDefault();
                if (app != null)
                {
                    var appender = app as RollingFileAppender;

                    directory = Path.GetDirectoryName(appender.File);

                    CleanUp(directory, logPrefix, date);
                }
            }

            /// <summary>
            /// Cleans up.
            /// </summary>
            /// <param name="logDirectory">The log directory.</param>
            /// <param name="logPrefix">The log prefix. Example: logfile dont include the file extension.</param>
            /// <param name="date">Anything prior will not be kept.</param>
            public void CleanUp(string logDirectory, string logPrefix, DateTime date)
            {
                if (string.IsNullOrEmpty(logDirectory))
                    throw new ArgumentException("logDirectory is missing");

                if (string.IsNullOrEmpty(logDirectory))
                    throw new ArgumentException("logPrefix is missing");

                var dirInfo = new DirectoryInfo(logDirectory);
                if (!dirInfo.Exists)
                    return;

                var fileInfos = dirInfo.GetFiles(string.Format("{0}*.*", logPrefix));
                if (fileInfos.Length == 0)
                    return;

                foreach (var info in fileInfos)
                {
                    if (string.Compare(info.Name, logPrefix, true) != 0)
                    {
                        if (info.CreationTime < date)
                        {
                            info.Delete();
                        }
                    }
                }

            }
            #endregion
        }

        private static void ClearOldLogs()
        {
            try
            {
                // Cleanup logs older than 5 days from http://stackoverflow.com/a/2916628/1067721
                var date = DateTime.Now.AddDays(-5);
                var task = new LogFileCleanupTask();
                task.CleanUp(date);
                // Cleanup service logs
                task.CleanUp("daas_assessment_tool.log", date);
            }
            catch (Exception) { }
        }

        public static void ShutdownLog()
        {
            LOG.Info("Shutting down logging system.");

            LogManager.Shutdown();
        }

        public static void InitializeLog()
        {
#if DEBUG
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("AssessmentLibrary.Resources.log4net_debug.xml"))
#else
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("AssessmentLibrary.Resources.log4net_release.xml"))
#endif            
            {
                log4net.Config.XmlConfigurator.Configure(stream);
            }
            ClearOldLogs();
        }
    }
}