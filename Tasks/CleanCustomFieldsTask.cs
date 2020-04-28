using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using MantisDBCleaner.Connection;
using MantisDBCleaner.Types;

namespace MantisDBCleaner.Tasks
{
    class CleanCustomFieldsTask : DatabaseTask
    {
        public override string Description => "Cleaning custom fields";

        protected override void ExecuteCommand(MantisDatabase db, DbCommand cmd, CleanOptions options, TaskDataStore store)
        {
            List<int> projectids = db.ReadList<int>($"SELECT id FROM {db.Table(MantisTable.project)}");
            
            // delete assignbment snon-existent projects
            db.DeleteMatching("custom field project assignments", $"DELETE FROM {db.Table(MantisTable.custom_field_project)} WHERE project_id NOT IN @@ids", projectids);

            // now delete all custom fields which do not have any assignments anymore
            List<int> customfieldids = db.ReadList<int>($"SELECT DISTINCT field_id FROM {db.Table(MantisTable.custom_field_project)}");

            db.DeleteMatching("custom fields", $"DELETE FROM {db.Table(MantisTable.custom_field)} WHERE id NOT IN @@ids", customfieldids);
            db.DeleteMatching("custom field strings", $"DELETE FROM {db.Table(MantisTable.custom_field_string)} WHERE field_id NOT IN @@ids", customfieldids);
        }
    }
}
