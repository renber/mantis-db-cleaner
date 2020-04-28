using CommandLine;
using System;
using System.Collections.Generic;
using System.Text;

namespace MantisDBCleaner
{
    class Options : CleanOptions
    {
        [Option('h', "host", Required = true, HelpText = "Database host")]
        public string Host { get; set; }

        [Option('d', "database", Required = true, HelpText = "Database name")]
        public string Database { get; set; }

        [Option('u', "user", Required = true, HelpText = "Database user")]
        public string Username { get; set; }

        [Option('p', "password", HelpText = "Database user's password")]
        public string Password { get; set; } = "";

        [Option("table-prefix", HelpText = "Table name prefix")]
        public string TablePrefix { get; set; } = "mantis_";

        [Option("commit-changes", Default = false, HelpText = "Write changes to the database (only supported for transactional database engines). If not specified changes are rolled backed.")]
        public bool CommitChanges { get; set; }

    }

    class CleanOptions
    {
        [Option("keep-projects", HelpText = "Names of the projects to keep in the database")]
        public IEnumerable<string> ProjectNamesToKeep { get; set; }
    }   
}
