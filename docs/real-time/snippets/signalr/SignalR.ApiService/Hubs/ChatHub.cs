public sealed class ChatHub: Hub
{
    public Task PostMessage(UserMessage userMessage) =>
        Clients.All.SendAsync(
            HubEventNames.MessageReceived, userMessage);

    public Task ToggleUserTyping(UserAction userAction) =>
        Clients.Others.SendAsync(
            HubEventNames.UserTypingChanged, userAction);
}
