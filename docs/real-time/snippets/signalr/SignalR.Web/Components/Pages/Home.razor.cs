using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using SignalR.Shared;
using SignalR.Web.Extensions;

namespace SignalR.Web.Components.Pages;

public sealed partial class Home : IAsyncDisposable
{
    private HubConnection? _hubConnection;
    private HashSet<UserMessage> _userMessages = [];
    private readonly HashSet<IDisposable> _hubRegistrations = [];

    [Inject]
    public required IConfiguration Configuration { get; set; }

    [Inject]
    public required NavigationManager Nav { get; set; }

    [Inject]
    public required IJSRuntime JavaScript { get; set; }

    protected override async Task OnInitializedAsync()
    {
        var api = Configuration.GetServiceHttpUri("apiservice");

        var builder = new UriBuilder(api)
        {
            Path = KnownChatHub.Endpoint
        };

        _hubConnection = new HubConnectionBuilder()
            .WithUrl(builder.Uri)
            .WithAutomaticReconnect()
            .Build();

        _hubRegistrations.Add(
            _hubConnection.On<UserMessage>(
                KnownChatHub.EventNames.MessageReceived, OnMessageReceivedAsync));

        await _hubConnection.StartAsync();
    }

    public Task OnMessageReceivedAsync(UserMessage userMessage) =>
        InvokeAsync(async () =>
        {
            _userMessages.Add(userMessage);

            await JavaScript.InvokeVoidAsync("app.updateScroll");

            StateHasChanged();
        });

    public async ValueTask DisposeAsync()
    {
        if (_hubRegistrations is { Count: > 0 })
        {
            foreach (var disposable in _hubRegistrations)
            {
                disposable.Dispose();
            }
        }

        if (_hubConnection is not null)
        {
            await _hubConnection.DisposeAsync();
        }
    }
}
