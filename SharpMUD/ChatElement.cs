namespace SharpMUD
{
    public abstract class ChatElement
    {
        public string ID { get; protected set; }
        public double Timestamp { get; protected set; }

        public string To { get; protected set; }
        public string From { get; protected set; }
        public string Channel { get; protected set; }

        public ChatElement(string id, double timestamp, string to, string from, string channel)
        {
            ID = id;
            Timestamp = timestamp;

            To = to;
            From = from;
            Channel = channel;
        }
    }
}
