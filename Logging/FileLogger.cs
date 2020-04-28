using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MantisDBCleaner.Logging
{
    public class FileLogger : ILogWriter, IDisposable
    {
        public String Filename { get; set; }

        private StreamWriter writer;

        public FileLogger(String filename)
        {
            try
            {
                writer = new StreamWriter(filename);
            }
            catch (Exception e)
            {
                LogService.Error("Could not create FileLogger: " + e.Message);
                writer = null;
            }
        }
        public void Log(LogLevel logLevel, string message)
        {
            if (writer != null)
            {
                writer.WriteLine(DateTime.Now.ToString() + " " + logLevel.ToReadableString() + " " + message);
                writer.Flush();
            }
        }
        public void Dispose()
        {
            if (writer != null)
            {
                writer.Close();
                writer = null;
            }
        }
    }
}
