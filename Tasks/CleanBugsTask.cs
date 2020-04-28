using MantisDBCleaner.Connection;
using MantisDBCleaner.Logging;
using MantisDBCleaner.Types;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Runtime.CompilerServices;
using System.Text;

namespace MantisDBCleaner.Tasks
{
    class CleanBugsTask : DatabaseTask
    {
        public override string Description => "Cleaning BugNotes";

        protected override void ExecuteCommand(MantisDatabase db, DbCommand cmd, CleanOptions options, TaskDataStore store)
        {
            // delete bugs which belong to non-existent projects
            List<int> projectids = store.Get<List<int>>(DataKey.KeepProjectIDs);
            db.DeleteMatching("bugs", $"DELETE FROM {db.Table(MantisTable.bug)} WHERE project_id NOT IN @@ids", projectids);

            // we cannot use DELETE FROM WHERE [id] IN (SELECT **), see https://dba.stackexchange.com/questions/84805/delete-seems-to-hang
            // or multiple-table DELETES because they always time out
            // get the list of bug_ids beforehand and list them separately
            // todo: if tehre are too many ids (command gets too long), we might need to delete in batches

            // delete all bug_texts which are not used anymore
            List<int> bugtextids = db.ReadList<int>($"SELECT bug_text_id FROM {db.Table(MantisTable.bug)}");
            db.DeleteMatching("bug text entries", $"DELETE FROM {db.Table(MantisTable.bug_text)} WHERE id NOT IN @@ids", bugtextids);

            // delete all bug_notes which reference non-existent bugs
            List<int> bugids = db.ReadList<int>($"SELECT id FROM {db.Table(MantisTable.bug)}");
            db.DeleteMatching("bug notes", $"DELETE FROM {db.Table(MantisTable.bugnote)} WHERE bug_id NOT IN @@ids", bugids);

            // delete all bug_note_texts which reference non-existent bug notes
            List<int> notetextids = db.ReadList<int>($"SELECT bugnote_text_id FROM {db.Table(MantisTable.bugnote)}");
            db.DeleteMatching("bug note text entries", $"DELETE FROM {db.Table(MantisTable.bugnote_text)} WHERE id NOT IN @@ids", notetextids);

            // delete all bug files which reference non-existent bugs (takes some time)
#if RELEASE
            int oldTimeout = cmd.CommandTimeout;
            cmd.CommandTimeout = (int)TimeSpan.FromSeconds(30).TotalMilliseconds;
            db.DeleteMatching("bug attached files", $"DELETE FROM {db.Table(MantisTable.bug_file)} WHERE bug_id NOT IN @@ids", bugids);
            cmd.CommandTimeout = oldTimeout;
#endif

            // delete all bug history entries which reference non-existent bugs
            db.DeleteMatching("bug history entries", $"DELETE FROM {db.Table(MantisTable.bug_history)} WHERE bug_id NOT IN @@ids", bugids);

            // delete all bug monitor entries which reference non-existent bugs
            db.DeleteMatching("bug monitor entries", $"DELETE FROM {db.Table(MantisTable.bug_monitor)} WHERE bug_id NOT IN @@ids", bugids);

            // delete all bug relationships which reference non-existent bugs
            db.DeleteMatching("bug relationships", $"DELETE FROM {db.Table(MantisTable.bug_relationship)} WHERE source_bug_id NOT IN @@ids OR destination_bug_id NOT IN @@ids", bugids);

            // delete all bug revision entries which reference non-existent bugs
            db.DeleteMatching("bug revision entries", $"DELETE FROM {db.Table(MantisTable.bug_revision)} WHERE bug_id NOT IN @@ids", bugids);

            // delete all bug tags which reference non-existent bugs
            db.DeleteMatching("bug tags", $"DELETE FROM {db.Table(MantisTable.bug_tag)} WHERE bug_id NOT IN @@ids", bugids);
        }



    }
}
