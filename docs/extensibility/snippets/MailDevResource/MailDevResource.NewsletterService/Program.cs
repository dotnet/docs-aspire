using MailKit.Client;
using Microsoft.AspNetCore.Mvc;
using MimeKit;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add services to the container.
// <smtp>
builder.AddMailDevClient("maildev");
// </smtp>

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();

// <subs>
app.MapPost("/subscribe", async ([FromServices] MailKitClientFactory factory, string email) =>
{
    var client = await factory.GetSmtpClientAsync();

    var message = new MimeMessage
    {
        Subject = "Welcome to our newsletter!",
        Body = new TextPart("plain")
        {
            Text = "Thank you for subscribing to our newsletter!"
        },
        From = { new MailboxAddress("Dev Newsletter", "newsletter@yourcompany.com") },
        To = { new MailboxAddress("Recipient Name", email) }
    };

    await client.SendAsync(message);
});

app.MapPost("/unsubscribe", async ([FromServices] MailKitClientFactory factory, string email) =>
{
    var client = await factory.GetSmtpClientAsync();

    var message = new MimeMessage
    {
        Subject = "You are unsubscribed from our newsletter!",
        Body = new TextPart("plain")
        {
            Text = "Sorry to see you go. We hope you will come back soon!"
        },
        From = { new MailboxAddress("Dev Newsletter", "newsletter@yourcompany.com") },
        To = { new MailboxAddress("Recipient Name", email) }
    };

    await client.SendAsync(message);
});
// </subs>

app.Run();
