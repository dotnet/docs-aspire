using MailDev.Client;
using Microsoft.AspNetCore.Mvc;

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
