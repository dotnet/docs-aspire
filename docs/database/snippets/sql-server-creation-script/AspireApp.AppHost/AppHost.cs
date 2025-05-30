var builder = DistributedApplication.CreateBuilder(args);

var sql = builder.AddSqlServer("sql")
                 .WithLifetime(ContainerLifetime.Persistent);

var databaseName = "app-db";
var creationScript = $$"""
    IF DB_ID('{{databaseName}}') IS NULL
        CREATE DATABASE [{{databaseName}}];
    GO

    -- Use the database
    USE [{{databaseName}}];
    GO

    -- Create the todos table
    CREATE TABLE todos (
        id INT PRIMARY KEY IDENTITY(1,1),        -- Unique ID for each todo
        title VARCHAR(255) NOT NULL,             -- Short description of the task
        description TEXT,                        -- Optional detailed description
        is_completed BIT DEFAULT 0,              -- Completion status
        due_date DATE,                           -- Optional due date
        created_at DATETIME DEFAULT GETDATE()    -- Creation timestamp
    );
    GO

    """;

var db = sql.AddDatabase(databaseName)
            .WithCreationScript(creationScript);

builder.AddProject<Projects.AspireApp_ExampleProject>("exampleproject")
       .WithReference(db)
       .WaitFor(db);

// After adding all resources, run the app...

builder.Build().Run();
