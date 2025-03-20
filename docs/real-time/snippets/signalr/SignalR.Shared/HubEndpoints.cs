namespace SignalR.Shared;

public static class HubEndpoints
{
    public const string ChatHub = $"/{ChatHubWithoutRouteSlash}";

    public const string ChatHubWithoutRouteSlash = "chathub";
}

public static class HubClientMethodNames
{
    public const string PostMessage = nameof(PostMessage);

    public const string ToggleUserTyping = nameof(ToggleUserTyping);
}

public static class HubEventNames
{
    public const string UserTypingChanged = nameof(UserTypingChanged);

    public const string MessageReceived = nameof(MessageReceived);
}
