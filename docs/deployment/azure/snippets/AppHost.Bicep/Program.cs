var builder = DistributedApplication.CreateBuilder(args);

var storage = builder.AddBicepTemplate("storage", "storage.bicep");

var user = builder.AddBicepTemplateString("user", """
    var user = {
      'user-name': 'Test Person'
    }

    output userName string = user['user-name']
    """);

var userName = user.GetOutput("userName");



builder.Build().Run();
