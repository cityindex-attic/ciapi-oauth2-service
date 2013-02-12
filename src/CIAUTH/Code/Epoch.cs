using System;

namespace CIAUTH.Code
{
    public static class Epoch
    {
        public static long ToEpoch(this DateTime time)
        {
            return (long)(time.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalSeconds;
        }

        public static DateTime FromEpoch(long epoch)
        {
            var d = new DateTime(1970, 1, 1);
            d = d.AddSeconds(epoch);
            return d.ToLocalTime();
        }
    }
}