using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MantisDBCleaner.Logging
{
    public interface ILogWriter
    {
        void Log(LogLevel logLevel, String message);
    }
}
