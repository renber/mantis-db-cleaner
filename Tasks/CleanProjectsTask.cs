using MantisDBCleaner.Connection;
using MantisDBCleaner.Logging;
using MantisDBCleaner.Types;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace MantisDBCleaner.Tasks
{
    class CleanProjectsTask : DatabaseTask
    {
        public override string Description => "Cleaning project table";

        protected override void ExecuteCommand(MantisDatabase db, DbCommand cmd, CleanOptions options, TaskDataStore store)
        {
            List<int> projectids = store.Get<List<int>>(DataKey.KeepProjectIDs);
            db.DeleteMatching("projects", $"DELETE FROM {db.Table(MantisTable.project)} WHERE id NOT IN @@ids", projectids);

            db.DeleteMatching("project files", $"DELETE FROM {db.Table(MantisTable.project_file)} WHERE project_id NOT IN @@ids", projectids);
            db.DeleteMatching("project hierarchy entries", $"DELETE FROM {db.Table(MantisTable.project_hierarchy)} WHERE parent_id NOT IN @@ids OR child_id NOT IN @@ids", projectids);
            db.DeleteMatching("project user assignments", $"DELETE FROM {db.Table(MantisTable.project_user_list)} WHERE project_id NOT IN @@ids", projectids);
            db.DeleteMatching("project versions", $"DELETE FROM {db.Table(MantisTable.project_version)} WHERE project_id NOT IN @@ids", projectids);
        }
    }
}
