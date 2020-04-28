# mantis-db-cleaner
Program which cleans a Mantis BugTracker database (copy) by only keeping data relevant to specific projects and deleting personal/confidential data.

**Do not run this on your production database**

# Command line parameters
-h, --host          Required. Database host

-d, --database      Required. Database name

-u, --user          Required. Database user

-p, --password      Database user's password

--table-prefix      (Default: mantis_) Table name prefix

--commit-changes    (Default: false) Write changes to the database (only supported for transactional database
                  engines). If not specified changes are rolled backed.

--keep-projects     Names of the projects to keep in the database

--help              Display this help screen.

--version           Display version information.
