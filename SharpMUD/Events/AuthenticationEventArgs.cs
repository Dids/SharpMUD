using System;

namespace SharpMUD.Events
{
    public class AuthenticationEventArgs : EventArgs
    {
        public string Token { get; private set; }
        
        public AuthenticationEventArgs(string token)
        {
            Token = token;
        }
    }
}
