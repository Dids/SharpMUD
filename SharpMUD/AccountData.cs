using System.Collections.Generic;
using System.Linq;

namespace SharpMUD
{
    public class AccountData
    {
        public List<User> Users { get; }

        public AccountData()
        {
            Users = new List<User>();
        }

        public User FindUser(string name)
        {
            try
            {
                return Users.First(x => x.Name == name);
            }
            catch
            {
                return null;
            }
        }
    }
}
