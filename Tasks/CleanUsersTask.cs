using MantisDBCleaner.Connection;
using MantisDBCleaner.Logging;
using MantisDBCleaner.Types;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace MantisDBCleaner.Tasks
{
    class CleanUsersTask : DatabaseTask
    {
        public override string Description => "Cleaning user data";

        protected override void ExecuteCommand(MantisDatabase db, DbCommand cmd, CleanOptions options, TaskDataStore store)
        {
            // delete all users which are not referenced anymore
            List<int> userids = db.ReadList<int>($"SELECT DISTINCT reporter_id FROM {db.Table(MantisTable.bugnote)}");
            userids.Union(db.ReadList<int>($"SELECT DISTINCT user_id FROM {db.Table(MantisTable.bug_file)}"));
            userids.Union(db.ReadList<int>($"SELECT DISTINCT user_id FROM {db.Table(MantisTable.bug_history)}"));
            userids.Union(db.ReadList<int>($"SELECT DISTINCT user_id FROM {db.Table(MantisTable.bug_monitor)}"));
            userids.Union(db.ReadList<int>($"SELECT DISTINCT user_id FROM {db.Table(MantisTable.bug_revision)}"));
            userids.Union(db.ReadList<int>($"SELECT DISTINCT reporter_id FROM {db.Table(MantisTable.bug)}"));
            userids.Union(db.ReadList<int>($"SELECT DISTINCT handler_id FROM {db.Table(MantisTable.bug)}"));
            userids.Union(db.ReadList<int>($"SELECT DISTINCT user_id FROM {db.Table(MantisTable.bug_tag)}"));
            userids.Union(db.ReadList<int>($"SELECT DISTINCT user_id FROM {db.Table(MantisTable.category)}"));
            userids.Union(db.ReadList<int>($"SELECT DISTINCT user_id FROM {db.Table(MantisTable.config)}"));
            userids.Union(db.ReadList<int>($"SELECT DISTINCT user_id FROM {db.Table(MantisTable.filters)}"));
            userids.Union(db.ReadList<int>($"SELECT DISTINCT poster_id FROM {db.Table(MantisTable.news)}"));
            userids.Union(db.ReadList<int>($"SELECT DISTINCT user_id FROM {db.Table(MantisTable.project_file)}"));
            userids.Union(db.ReadList<int>($"SELECT DISTINCT user_id FROM {db.Table(MantisTable.project_user_list)}"));
            userids.Union(db.ReadList<int>($"SELECT DISTINCT user_id FROM {db.Table(MantisTable.sponsorship)}"));
            userids.Union(db.ReadList<int>($"SELECT DISTINCT user_id FROM {db.Table(MantisTable.tag)}"));
            userids.Union(db.ReadList<int>($"SELECT DISTINCT user_id FROM {db.Table(MantisTable.user_profile)}"));

            // remove all users which are not referenced anymore
            db.DeleteMatching("users", $"DELETE FROM {db.Table(MantisTable.user)} WHERE id NOT IN @@ids", userids);
            db.DeleteMatching("user profiles", $"DELETE FROM {db.Table(MantisTable.user_profile)} WHERE user_id NOT IN @@ids", userids);

            // Remove confidential/personal user data
            cmd.CommandText = $"UPDATE {db.Table(MantisTable.user)} SET email='redacted',password='redacted',login_count=0,lost_password_request_count=0,failed_login_count=0,cookie_string=CONCAT('redacted_', id),last_visit=0,date_created=0";
            int count = cmd.ExecuteNonQuery();
            LogService.Info($"Removed personal data from all remaining {count} user profiles");

            // delete personal data
            db.DeleteAll("user preferences", MantisTable.user_pref);
            db.DeleteAll("user print preferences", MantisTable.user_print_pref);
        }
    }
}
