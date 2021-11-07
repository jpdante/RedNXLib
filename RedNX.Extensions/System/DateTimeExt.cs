using System;

namespace RedNX.Extensions.System {
    public static class DateTimeExt {

        public static int ToUnixEpoch(this DateTime dateTime) {
            var currTime = dateTime - DateTime.UnixEpoch;
            return Convert.ToInt32(Math.Abs(currTime.TotalSeconds));
        }

        public static DateTime FromUnixEpoch(int time) {
            return DateTime.UnixEpoch.AddSeconds(Convert.ToDouble(time));
        }

    }
}