using System.Collections.Generic;

namespace SharpMUD
{
    public class AccountData
    {
        public List<User> Users { get; }

        public AccountData()
        {
            Users = new List<User>();
        }
    }
}
