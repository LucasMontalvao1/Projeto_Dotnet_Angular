using System;

namespace ApiWeb.Helpers
{
    public static class DateHelper
    {
        public static int CalculateDateDifference(DateTime startDate, DateTime endDate)
        {
            return (endDate.Date - startDate.Date).Days;
        }

        public static bool IsValidTransactionDate(DateTime date)
        {
            return date.Date <= DateTime.Now.Date;
        }

        public static DateTime GetFirstDayOfMonth(DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1);
        }

        public static DateTime GetLastDayOfMonth(DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1).AddMonths(1).AddDays(-1);
        }
    }
}