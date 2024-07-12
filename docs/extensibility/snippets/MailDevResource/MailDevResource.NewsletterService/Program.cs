using MailDev.Client;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
// <smtp>
builder.AddMailDevClient("maildev");
// </smtp>

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

// <subs>
app.MapPost("/subscribe", async ([FromServices] MailDevClient client, string email) =>
{
    await client.SubscribeToNewsletterAsync(email);
});

app.MapPost("/unsubscribe", async ([FromServices] MailDevClient client, string email) =>
{
    await client.UnsubscribeToNewsletterAsync(email);
});
// </subs>

app.Run();
