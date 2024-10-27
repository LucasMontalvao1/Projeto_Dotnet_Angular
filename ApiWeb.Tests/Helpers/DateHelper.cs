namespace ApiWeb.Helpers
{
    public static class DateHelper
    {
        public static int CalculateDateDifference(DateTime startDate, DateTime endDate)
        {
            return (endDate - startDate).Days;
        }
    }
}
