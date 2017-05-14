using System;

namespace SharpMUD
{
    public class Utilities
    {
        public static double GetTime()
        {
            var st = new DateTime(1970, 1, 1);
            TimeSpan t = (DateTime.Now.ToUniversalTime() - st);
            return t.TotalMilliseconds;
        }
    }
}
