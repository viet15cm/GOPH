using System.Globalization;
using System;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GOPH.Extensions
{
    public static class ConvertDatetime
    {
        public static string GetConvert(DateTime dateTimeNow , DateTime curentDatetime)
        {
            if (dateTimeNow < curentDatetime)
            {
                return null;
            }

            var data = dateTimeNow - curentDatetime;

            if (data.Days <= 30 && data.Days > 0)
            {
                return $"{data.Days} ngày trước";
            }
            else if (data.Hours < 24 && data.Hours > 0)
            {
                return $"{data.Hours} giờ trước";
            }
            else if (data.Minutes < 60 && data.Minutes > 0)
            {
                return $"{data.Minutes} phút trước";

            }
            else if (data.Seconds < 60 && data.Seconds > 0)
            {
                return $"{data.Seconds} giây trước";
            }
          
             return curentDatetime.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

        }

        public static string GetDateTime(DateTime dateTime)
        {
            return dateTime.ToString("dd/MM/yyyy hh:mm tt", CultureInfo.InvariantCulture);
        }

        public static string GetDate(DateTime dateTime) 
        {
            return dateTime.ToString("dd/MM/yyyy ", CultureInfo.InvariantCulture);
        }

        public static string GetTime(DateTime dateTime)
        {
            return  dateTime.ToString("hh:mm tt");
        }

        public static DateTime CurentDateTimeVN()
        {
            DateTime utcDateTime = DateTime.UtcNow;
            string vnTimeZoneKey = "SE Asia Standard Time";
            TimeZoneInfo vnTimeZone = TimeZoneInfo.FindSystemTimeZoneById(vnTimeZoneKey);
            return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, vnTimeZone);
        }
    }
}
