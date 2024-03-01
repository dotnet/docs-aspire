using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using SignalR.Shared;
using SignalR.Web.Extensions;

namespace SignalR.Web.Components.Pages;

public sealed partial class Home : IAsyncDisposable
{
    private enum ConnectionStatus { Connecting, Connected, Disconnected, Reconnecting }

    private ConnectionStatus _connectionStatus = ConnectionStatus.Connecting;
    private string _username = Guid.NewGuid().ToString();
    private string _message = string.Empty;

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
            Path = HubEndpoints.ChatHub
        };

        _hubConnection = new HubConnectionBuilder()
            .WithUrl(builder.Uri)
            .WithAutomaticReconnect()
            .Build();

        _hubConnection.Closed += OnHubConnectionClosedAsync;
        _hubConnection.Reconnected += OnHubConnectionReconnectedAsync;
        _hubConnection.Reconnecting += OnHubConnectionReconnectingAsync;

        _hubRegistrations.Add(
            _hubConnection.On<UserMessage>(
                HubEventNames.MessageReceived, OnMessageReceivedAsync));

        await _hubConnection.StartAsync();

        _connectionStatus = ConnectionStatus.Connected;
    }

    private Task OnHubConnectionReconnectingAsync(Exception? arg) =>
        InvokeAsync(() =>
        {
            _connectionStatus = ConnectionStatus.Reconnecting;
            StateHasChanged();
        });

    private Task OnHubConnectionReconnectedAsync(string? arg) =>
        InvokeAsync(() =>
        {
            _connectionStatus = ConnectionStatus.Connected;
            StateHasChanged();
        });

    private Task OnHubConnectionClosedAsync(Exception? arg) =>
        InvokeAsync(() =>
        {
            _connectionStatus = ConnectionStatus.Disconnected;
            StateHasChanged();
        });

    public Task OnMessageReceivedAsync(UserMessage userMessage) =>
        InvokeAsync(async () =>
        {
            _userMessages.Add(userMessage);

            await JavaScript.InvokeVoidAsync("app.updateScroll");

            StateHasChanged();
        });

    public async Task TryPostMessageAsync()
    {
        if (string.IsNullOrWhiteSpace(_username) ||
            string.IsNullOrWhiteSpace(_message))
        {
            return;
        }

        if (_hubConnection is null)
        {
            return;
        }

        await _hubConnection.SendAsync(
            HubClientMethodNames.PostMessage,
            new UserMessage(_username, _message));
    }

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
            _hubConnection.Closed -= OnHubConnectionClosedAsync;
            _hubConnection.Reconnected -= OnHubConnectionReconnectedAsync;
            _hubConnection.Reconnecting -= OnHubConnectionReconnectingAsync;

            await _hubConnection.DisposeAsync();
        }
    }
}
