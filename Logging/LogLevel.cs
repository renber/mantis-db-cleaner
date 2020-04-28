using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MantisDBCleaner.Logging
{
    public enum LogLevel
    {
        Critical = 0,
        Error,
        Warning,
        Information,
        Debug,
        Trace
    }

    public static class LogLevelExtensions
    {
        public static string ToReadableString(this LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Critical: return "[CRITICAL]";
                case LogLevel.Error: return "[ERROR]";
                case LogLevel.Warning: return "[WARNING]";
                case LogLevel.Information: return "[INFO]";
                case LogLevel.Debug: return "[DEBUG]";
                case LogLevel.Trace: return "[TRACE]";
                default: return "[UNKNOWN]";
            }
        }
    }
}
