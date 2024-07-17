var builder = DistributedApplication.CreateBuilder(args);

var maildev = builder.AddMailDev("maildev");

builder.AddProject<Projects.MailDevResource_NewsletterService>("newsletterservice")
       .WithReference(maildev);

builder.Build().Run();
