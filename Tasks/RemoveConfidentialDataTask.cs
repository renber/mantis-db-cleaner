using MantisDBCleaner.Connection;
using MantisDBCleaner.Types;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace MantisDBCleaner.Tasks
{
    /// <summary>
    /// Remove personal or confidential information from the database
    /// </summary>
    class RemoveConfidentialDataTask : DatabaseTask
    {
        public override string Description => "Removing confidential data";

        protected override void ExecuteCommand(MantisDatabase db, DbCommand cmd, CleanOptions options, TaskDataStore store)
        {
            db.DeleteAll("api tokens", MantisTable.api_token);
            db.DeleteAll("tokens", MantisTable.tokens);
            db.DeleteAll("emails", MantisTable.email);
            db.DeleteAll("filters", MantisTable.filters);
            db.DeleteAll("news", MantisTable.news);
            db.DeleteAll("sponsorship entries", MantisTable.sponsorship);            
        }
    }
}
