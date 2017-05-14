namespace SharpMUD
{
    public class Presence : ChatElement
    {
        public bool Joined { get; private set; }

        public Presence(string id, double timestamp, string to, string from, string channel, bool joined) : base(id, timestamp, to, from, channel)
        {
            Joined = joined;
        }
    }
}
