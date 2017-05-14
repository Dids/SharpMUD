namespace SharpMUD.API
{
    internal static class Methods
    {
        internal static string GetToken => Common.CombineMethodLink("get_token");
        internal static string AccountData => Common.CombineMethodLink("account_data");
        internal static string ChatHistory => Common.CombineMethodLink("chats");
        internal static string MessageOutlet => Common.CombineMethodLink("create_chat");
    }
}
