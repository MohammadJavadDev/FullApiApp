using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Utilities
{
    public static class DateTimeExtentions
    {
        public static string ToShamsiDateTime(this DateTime date)

        {
            PersianCalendar pc = new PersianCalendar();

            return  string.Format("{0}/{1}/{2} {3}:{4}:{5}",
                      pc.GetYear(date),
                      pc.GetMonth(date),
                      pc.GetDayOfMonth(date),
                      pc.GetHour(date),
                      pc.GetMinute(date),
                      pc.GetSecond(date));

             

        }
        public static string ToShamsiDate(this DateTime date)

        {
            PersianCalendar pc = new PersianCalendar();

            return string.Format("{0}/{1}/{2}",
                      pc.GetYear(date),
                      pc.GetMonth(date),
                      pc.GetDayOfMonth(date)
                      );



        }

        public static DateTime ToMiladiDate(this string date)

        {
            PersianCalendar pc = new PersianCalendar();
            try {

                var year = date.Substring(0, 4).ToInt();
                var mount = date.Substring(5, 2).ToInt();
                var day = date.Substring(8, 2).ToInt();
                return pc.ToDateTime(year, mount, day, 0, 0, 0, 0);
            }
            catch(Exception ex)
            {
                throw new Exception("Error on ConvertDate ToMiladi " + ex.Message);
            }
        }

        public static DateTime ToMiladiDateTime(this string date)

        {
            PersianCalendar pc = new PersianCalendar();
            try
            {

                var year = date.Substring(0, 4).ToInt();
                var mount = date.Substring(5, 2).ToInt();
                var day = date.Substring(8, 2).ToInt();
                var hour = date.Substring(11, 2).ToInt();
                var min = date.Substring(14, 2).ToInt();
                var sec = date.Substring(17, 2).ToInt();
                return pc.ToDateTime(year, mount, day, hour, min, sec, 0);
            }
            catch (Exception ex)
            {
                throw new Exception("Error on ConvertDateTime ToMiladi " + ex.Message);
            }
        }


    }
}
