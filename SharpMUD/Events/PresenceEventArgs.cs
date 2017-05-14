namespace SharpMUD.Events
{
    public class PresenceEventArgs
    {
        public Presence Presence { get; private set; }

        public PresenceEventArgs(Presence presence)
        {
            Presence = presence;

        }
    }
}
