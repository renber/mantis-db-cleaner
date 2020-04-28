using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MantisDBCleaner.Logging
{
    public static class LogService
    {

        private static IList<ILogWriter> loggers = new List<ILogWriter>();

        private static LogLevel _logLevel = LogLevel.Information;

        /// <summary>
        /// The log level
        /// Only messages whose log level is less (more severe) or equal to this level are logged
        /// </summary>
        public static LogLevel LogLevel
        {
            get
            {
                return _logLevel;
            }
            set
            {
                _logLevel = value;
            }
        }

        public static void RegisterLogger(ILogWriter logWriter)
        {
            loggers.Add(logWriter);
        }

        public static void UnregisterLogger(ILogWriter logWriter)
        {
            loggers.Remove(logWriter);
        }

        /// <summary>
        /// Log the message with the given log level
        /// </summary>
        /// <param name="logLevel"></param>
        /// <param name="message"></param>
        public static void Log(LogLevel logLevel, String message)
        {
            if ((int)logLevel <= (int)LogLevel)
            {
                foreach (var l in loggers)
                {
                    try
                    {
                        l.Log(logLevel, message);
                    } catch (Exception exc)
                    {
                        // defective logger?
                    }
                }                    
            }
        }

        /// <summary>
        /// Log the message with error level Critical
        /// </summary>        
        public static void Critical(String message)
        {
            Log(LogLevel.Critical, message);
        }

        /// <summary>
        /// Log the message with error level Error
        /// </summary>        
        public static void Error(String message)
        {
            Log(LogLevel.Error, message);
        }

        /// <summary>
        /// Log the message with error level Warning
        /// </summary>        
        public static void Warn(String message)
        {
            Log(LogLevel.Warning, message);
        }

        /// <summary>
        /// Log the message with error level Info
        /// </summary>        
        public static void Info(String message)
        {
            Log(LogLevel.Information, message);
        }

        /// <summary>
        /// Log the message with error level Debug
        /// </summary>        
        public static void Debug(String message)
        {
            Log(LogLevel.Debug, message);
        }

        /// <summary>
        /// Log the message with error level Trace
        /// </summary>        
        public static void Trace(String message)
        {
            Log(LogLevel.Trace, message);
        }

    }
}
