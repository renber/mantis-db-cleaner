using MantisDBCleaner.Connection;
using MantisDBCleaner.Logging;
using MantisDBCleaner.Types;
using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace MantisDBCleaner.Tasks
{
    class PrepareCleanTask : DatabaseTask
    {
        public override string Description => "Preparing database clean";

        protected override void ExecuteCommand(MantisDatabase db, DbCommand cmd, CleanOptions options, TaskDataStore store)
        {
            // get the id of the project to keep
            cmd.CommandText = $"SELECT id, name FROM {db.Table(MantisTable.project)} WHERE name IN @@projectnames";
            cmd.AddArrayParameters("@@projectnames", options.ProjectNamesToKeep);

            List<int> keepIds = new List<int>();

            using (var dr = cmd.ExecuteReader())
            {
                while (dr.Read())
                {
                    keepIds.Add(dr.Get<int>("id"));
                    LogService.Info($"Project '{dr.Get<String>("name")}' with id {dr.Get<int>("id")} will be kept");
                }
            }

            if (options.ProjectNamesToKeep.Count() != keepIds.Count)
            {
                throw new TaskFailedException("Not all mentioned projects werde found in the database");
            }

            store.Set(DataKey.KeepProjectIDs, keepIds);
        }
    }
}
