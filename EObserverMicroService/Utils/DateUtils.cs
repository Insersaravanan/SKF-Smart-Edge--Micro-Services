using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EMaintanance.Utils
{
    public static class DateUtils
    {

        static DateUtils()
        {

        }

        /** The below method is used to return the list of dates between the date range with the specified interval */
        public static List<DateTime> GetDatesBetweenRange(DateTime startDate, DateTime endDate, int interval)
        {
            List<DateTime> betweenDates = new List<DateTime>();
            for (DateTime date = startDate; date <= endDate; date = date.AddDays(interval))
            {
                betweenDates.Add(date);
            }
            return betweenDates;
        }

        /** The below method is used to Convert String to Date */
        public static DateTime ConvertStringToDate(string strDate)
        {
            if (!DateTime.TryParse(strDate, out DateTime returnDate))
            {
                throw new CustomException("Error while Convert String to Date, Please Contact Support!!!", "Error", true, "Error while Convert String to Date, Please Contact Support!!!");
            }

            return returnDate;
        }

    }
}
