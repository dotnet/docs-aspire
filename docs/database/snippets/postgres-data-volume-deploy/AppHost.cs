var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddAzurePostgresFlexibleServer("postgres")
                      .RunAsContainer(container => 
                      {
                        container.WithDataVolume();
                      });

builder.Build().Run();