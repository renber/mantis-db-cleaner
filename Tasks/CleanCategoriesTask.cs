using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using MantisDBCleaner.Connection;
using MantisDBCleaner.Types;

namespace MantisDBCleaner.Tasks
{
    class CleanCategoriesTask : DatabaseTask
    {
        public override string Description => "Cleaning categories";

        protected override void ExecuteCommand(MantisDatabase db, DbCommand cmd, CleanOptions options, TaskDataStore store)
        {
            List<int> projectids = db.ReadList<int>($"SELECT id FROM {db.Table(MantisTable.project)}");
            db.DeleteMatching("categories", $"DELETE FROM {db.Table(MantisTable.category)} WHERE project_id NOT IN @@ids", projectids);
        }
    }
}
