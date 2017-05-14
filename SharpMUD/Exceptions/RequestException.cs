using System;
using System.Net.Http;

namespace SharpMUD.Exceptions
{
    public class RequestException : Exception
    {
        public HttpResponseMessage Response { get; } 

        public RequestException(string message, HttpResponseMessage response) : base(message)
        {
            Response = response;
        }
    }
}
