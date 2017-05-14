using System;

namespace SharpMUD.Events
{
    public class MessageEventArgs : EventArgs
    {
        public Message Message { get; private set; }

        public MessageEventArgs(Message message)
        {
            Message = message;
        }
    }
}
