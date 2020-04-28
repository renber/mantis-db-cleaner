using MantisDBCleaner.Connection;
using MantisDBCleaner.Logging;
using MantisDBCleaner.Types;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace MantisDBCleaner.Tasks
{
    class ConnectionTestTask : ITask
    {
        public string Description => "Testing connection";

        public void Run(MantisDatabase db, CleanOptions options, TaskDataStore store)
        {
            try
            {
                using (var cmd = db.CreateCommand())
                {
                    cmd.CommandText = $"SELECT value FROM {db.Table(MantisTable.config)} WHERE config_id = 'database_version'";
                    var vr = cmd.ExecuteScalar();
                    LogService.Info("Mantis database version is " + vr?.ToString() ?? "");
                }
            }
            catch (Exception e)
            {
                throw new TaskFailedException(false, "Connection test failed: " + e.Message);
            }
        }
    }
}
