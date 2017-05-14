using System;

namespace SharpMUD.Events
{
    public class AccountDataEventArgs : EventArgs
    {
        public AccountData AccountData { get; private set; }

        public AccountDataEventArgs(AccountData accountData)
        {
            AccountData = accountData;
        }
    }
}
