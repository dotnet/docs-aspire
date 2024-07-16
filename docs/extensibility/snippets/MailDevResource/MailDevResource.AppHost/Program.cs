var builder = DistributedApplication.CreateBuilder(args);

var mailDevUsername = builder.AddParameter("maildev-username");
var mailDevPassword = builder.AddParameter("maildev-password");

var maildev = builder.AddMailDev("maildev")
    // See https://maildev.github.io/maildev/#usage
    .WithEnvironment("MAILDEV_INCOMING_USER", mailDevUsername)
    .WithEnvironment("MAILDEV_INCOMING_PASS", mailDevPassword);

builder.AddProject<Projects.MailDevResource_NewsletterService>("newsletterservice")
       .WithReference(maildev);

builder.Build().Run();
