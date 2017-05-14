namespace SharpMUD
{
    public class Message : ChatElement
    {
        public string Body { get; private set; }

        public Message(string id, double timestamp, string to, string from, string channel, string body) : base(id, timestamp, to, from, channel)
        {
            Body = body;
        }
    }
}
