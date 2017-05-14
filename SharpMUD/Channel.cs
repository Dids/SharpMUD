using System.Collections.Generic;

namespace SharpMUD
{
    public class Channel
    { 
        public string Name { get; private set; }
        public List<string> Members { get; private set; }

        public Channel(string name)
        {
            Name = name;
            Members = new List<string>();
        }

        public bool MemberExists(string name) => Members.Exists(x => x == name);
    }
}
