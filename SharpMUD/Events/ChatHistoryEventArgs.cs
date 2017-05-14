using System;

namespace SharpMUD.Events
{
    public class ChatHistoryEventArgs : EventArgs
    {
        public ChatHistory History { get; private set; }

        public ChatHistoryEventArgs(ChatHistory history)
        {
            History = history;
        }
    }
}
