using MantisDBCleaner.Connection;
using MantisDBCleaner.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace MantisDBCleaner.Tasks
{
    interface ITask
    {
        string Description { get; }

        void Run(MantisDatabase db, CleanOptions options, TaskDataStore store);
    }
}
