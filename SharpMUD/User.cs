using System.Collections.Generic;

namespace SharpMUD
{
    public class User
    {
        public string Name { get; private set; }
        public List<Channel> Channels { get; private set; }

        public bool Update { get; set; }

        public User(string name)
        {
            Name = name;
            Channels = new List<Channel>();

            Update = true;
        }
    }
}
