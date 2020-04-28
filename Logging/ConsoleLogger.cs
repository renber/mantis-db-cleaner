using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MantisDBCleaner.Logging
{
    public class ConsoleLogger : ILogWriter
    {
        public String Filename { get; set; }        

        
        public void Log(LogLevel logLevel, string message)
        {
            if (logLevel == LogLevel.Error || logLevel == LogLevel.Critical)
            {
                var oldColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(DateTime.Now.ToString() + " " + logLevel.ToReadableString() + " " + message);
                Console.ForegroundColor = oldColor;
            } else
            {
                Console.WriteLine(DateTime.Now.ToString() + " " + logLevel.ToReadableString() + " " + message);
            }            
        }        
    }
}
