using MantisDBCleaner.Logging;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace MantisDBCleaner.Connection
{
    class MantisDatabase : IDisposable
    {        
        public Options Options { get; }

        private MySqlConnection sharedConnection;
        private MySqlTransaction transaction;

        public MantisDatabase(Options options)
        {
            Options = options;
        }  
        
        private MySqlConnection Open()
        {
            if (sharedConnection == null)
            {
                sharedConnection = new MySqlConnection($"Server={Options.Host};User ID={Options.Username};Password={Options.Password};Database={Options.Database}");
                sharedConnection.Open();
                transaction = sharedConnection.BeginTransaction();

                LogService.Info("Connected to database");
            }

            return sharedConnection;              
        }

        public DbCommand CreateCommand()
        {
            var conn = Open();
            return new MySqlCommand(conn, transaction);
        }

        public string Table(MantisTable table)
        {
            return Options.TablePrefix + table.ToString() + "_table";
        }

        public void Dispose()
        {
            if (transaction != null)
            {                                
                if (Options.CommitChanges)
                {
                    LogService.Info("Committing changes");
                    transaction.Commit();
                } else
                {
                    // TODO: check if this is possible on startup (Database engine needs to support transactions)

                    LogService.Info("Rolling back changes");
                    transaction.Rollback();
                }

                transaction = null;
            }

            if (sharedConnection != null)
            {
                sharedConnection.Dispose();
                sharedConnection = null;
            }
        }
    }
}
