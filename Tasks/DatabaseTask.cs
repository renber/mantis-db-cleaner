using MantisDBCleaner.Connection;
using MantisDBCleaner.Types;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace MantisDBCleaner.Tasks
{
    abstract class DatabaseTask : ITask
    {
        public abstract string Description { get; }

        public void Run(MantisDatabase db, CleanOptions options, TaskDataStore store)
        {
            ExecuteCommand(db, db.CreateCommand(), options, store);
        }

        protected abstract void ExecuteCommand(MantisDatabase db, DbCommand cmd, CleanOptions options, TaskDataStore store);
    }
}

