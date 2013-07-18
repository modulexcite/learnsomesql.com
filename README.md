# Interactive SQL

Learn the basics of SQL by writing SQL in the browser to complete tasks.
Get immediate feedback as to whether the query was right or not.

## Development

This project is developed with Visual Studio 2012.
Other versions might work, but I haven't tried them.

### Running

Warning: when starting up, the application will attempt to delete any existing tables.
Make sure that the database you use has nothing you want to keep.

There are two ways of getting the application running:

* A self-hosted version.
  Useful for development, and doesn't require IIS to be installed.
  Just run the `RedGate.Publishing.InteractiveSql.SelfHosted` project.
* On IIS/ASP.NET.
  Host the project `RedGate.Publishing.InteractiveSql.AspNet` as a normal
  ASP.NET application under IIS.

### Configuration

There are a number of values that need to be set in `appSettings`:

* `SqlServerInstance` -- the instance of SQL Server to use.
  For instance, `.\sql2008` means use a local instance called `sql2008`.
* `SqlServerDatabase` -- the name of the database to use.
  Make sure it doesn't contain anything you want to keep:
  the application will attempt to delete everything on startup.
* `SqlServerAdminUsername` -- the name of a user that has sufficient
  privileges to delete the tables in the specified database, create
  new tables, and insert rows.
* `SqlServerAdminPassword` -- the password for the user specified by
  `SqlServerAdminUsername`
* `SqlServerUnprivilegedUsername` -- the name of a user that has sufficient
  privileges to run `SELECT` queries on the specified database. Make sure the
  user has no other privileges.
* `SqlServerUnprivilegedPassword` -- the password for the user specified by
  `SqlServerUnprivilegedUsername`

## License

[BSD 2-Clause](http://opensource.org/licenses/BSD-2-Clause)