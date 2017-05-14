using System.Collections.Generic;
using System.Linq;

namespace SharpMUD
{
    public class ChatHistory
    {
        private Stack<ChatElement> Messages { get; set; }

        public bool IsEmpty => Messages.Count <= 0;
        public ChatElement First => Messages.First();
        public ChatElement Last => Messages.Last();

        public ChatHistory()
        {
            Messages = new Stack<ChatElement>();
        }

        public void Push(ChatElement chatElement)
        {
            Messages.Push(chatElement);
        }

        public ChatElement Pop()
        {
            return Messages.Pop();
        }
    }
}
