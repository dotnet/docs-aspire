When you want to explicitly provide the username and password, you can provide those as parameters. Consider the following alternative example:

```csharp
var username = builder.AddParameter("username", secret: true);
var password = builder.AddParameter("password", secret: true);

var nats = builder.AddNats("nats", userName: username, password: password);

var exampleProject = builder.AddProject<Projects.ExampleProject>()
                            .WithReference(nats);
```

For more information, see [External parameters](../../fundamentals/external-parameters.md).
