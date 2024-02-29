using Microsoft.AspNetCore.SignalR;
using SignalR.Shared;

public sealed class ChatHub: Hub
{
    public Task PostMessage(UserMessage userMessage) =>
        Clients.All.SendAsync(
            KnownChatHub.EventNames.MessageReceived, userMessage);

    public Task ToggleUserTyping(UserAction userAction) =>
        Clients.Others.SendAsync(
            KnownChatHub.EventNames.UserTypingChanged, userAction);
}
