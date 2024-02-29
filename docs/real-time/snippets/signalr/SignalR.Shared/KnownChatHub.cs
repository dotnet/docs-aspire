namespace SignalR.Shared;

public static class KnownChatHub
{
    public const string Endpoint = "/chathub";

    public static class ClientMethodNames
    {
        public const string PostMessage = nameof(PostMessage);

        public const string ToggleUserTyping = nameof(ToggleUserTyping);
    }

    public static class EventNames
    {
        public const string UserTypingChanged = nameof(UserTypingChanged);

        public const string MessageReceived = nameof(MessageReceived);
    }
}
