
namespace SignalR.Web.Components.Pages;

public sealed partial class Home : IAsyncDisposable
{
    private enum ConnectionStatus { Connecting, Connected, Disconnected, Reconnecting }

    private ConnectionStatus _connectionStatus = ConnectionStatus.Connecting;
    private string _username = string.Empty;
    private string _message = string.Empty;
    private bool _isTyping;
    private const string UsernameKey = "username";
    private HubConnection? _hubConnection;
    private readonly HashSet<UserMessage> _userMessages = [];
    private readonly HashSet<string> _usersTyping = [];
    private readonly HashSet<IDisposable> _hubRegistrations = [];
    private readonly DebounceTimer _debounceTimer = new()
    {
        Interval = 750,
        AutoReset = false
    };

    [Inject] public required IConfiguration Configuration { get; set; }

    [Inject] public required NavigationManager Nav { get; set; }

    [Inject] public required IJSRuntime JavaScript { get; set; }

    [Inject] public required ILocalStorageService Storage { get; set; }

    public Home()
    {
        _debounceTimer.Elapsed += OnDebounceTimerElapsedAsync;
    }

    private async void OnDebounceTimerElapsedAsync(object? _, ElapsedEventArgs e) =>
        await SetIsTypingAsync(false);

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _username = await Storage.GetItemAsync(UsernameKey)
                ?? Guid.NewGuid().ToString();
        }
    }

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

        _hubRegistrations.Add(
            _hubConnection.On<UserAction>(
                HubEventNames.UserTypingChanged, OnUserTypingAsync));


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

    private async Task InitiateDebounceUserIsTypingAsync()
    {
        _debounceTimer.Stop();
        _debounceTimer.Start();

        await SetIsTypingAsync(true);
    }

    private async Task OnUserTypingAsync(UserAction action) =>
        await InvokeAsync(() =>
        {
            _ = action.IsTyping
                ? _usersTyping.Add(action.Name)
                : _usersTyping.Remove(action.Name);

            StateHasChanged();
        });

    private Task SetIsTypingAsync(bool isTyping)
    {
        if (_isTyping && isTyping || string.IsNullOrWhiteSpace(_username))
        {
            return Task.CompletedTask;
        }

        return _hubConnection?.InvokeAsync(
            HubClientMethodNames.ToggleUserTyping,
            new UserAction(Name: _username, IsTyping: _isTyping = isTyping)) ?? Task.CompletedTask;
    }

    private async Task TryPostMessageAsync()
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

        await Storage.SetItemAsync(UsernameKey, _username);

        await _hubConnection.SendAsync(
            HubClientMethodNames.PostMessage,
            new UserMessage(_username, _message));

        _message = string.Empty;
    }

    private async Task OnKeyUpAsync(KeyboardEventArgs args)
    {
        if (args is { Key: "Enter" } and { Code: "Enter" })
        {
            await TryPostMessageAsync();
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_debounceTimer is { })
        {
            _debounceTimer.Elapsed -= OnDebounceTimerElapsedAsync;
            _debounceTimer.Stop();
            _debounceTimer.Dispose();
        }

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
