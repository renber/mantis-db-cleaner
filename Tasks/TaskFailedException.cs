using System;
using System.Collections.Generic;
using System.Text;

namespace MantisDBCleaner.Tasks
{
    class TaskFailedException : Exception
    {

        public bool ContinueExecution { get; }

        public TaskFailedException(bool continueExecution, string message)
            : base(message)
        {
            ContinueExecution = continueExecution;
        }

        public TaskFailedException(string message)
            : this(false, message)
        {

        }
    }
}
