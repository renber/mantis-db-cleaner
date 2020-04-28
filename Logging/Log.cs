using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MantisDBCleaner.Logging
{

    /// <summary>
    /// Bietet eine Schnittstelle für Logdateien
    /// </summary>
    public class Log
    {                       
        /// <summary>
        /// Schreibt eine Nachricht inklusive Zeitstempel in die Logdatei        
        /// </summary>
        /// <param name="msg"></param>
        public static void Write(String msg)
        {
            LogService.Info(msg);            
        }

    }
}
