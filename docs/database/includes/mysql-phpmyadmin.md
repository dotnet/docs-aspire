### Use PhpMyAdmin

When adding MySql resources to the `builder` with the `AddMySql` method, you can chain calls to `WithPhpMyAdmin` to add the [**phpmyadmin**](https://www.phpmyadmin.net/) container. This container is a cross-platform client for MySql databases, that serves a web-based admin dashboard. Consider the following example:

```csharp
var mysql = builder.AddMySql("mysql")
                      .WithPhpMyAdmin();

var mysqldb = mysql.AddDatabase("mysqldb");

var myService = builder.AddProject<Projects.MyService>()
                       .WithReference(mysqldb);
```
