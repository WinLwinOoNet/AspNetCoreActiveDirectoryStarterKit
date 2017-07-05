using System;

namespace Asp.Core.Extensions
{
	public static class DateExtensions
	{
		public static DateTime ToStartOfMonth(this DateTime date)
		{
			return new DateTime(date.Year, date.Month, 1);
		}

		public static DateTime ToEndOfMonth(this DateTime date)
		{
			return new DateTime(date.Year, date.Month, 1).AddMonths(1).AddDays(-1);
		}

		public static DateTime ToEndOfDay(this DateTime date)
		{
			return date.Date.AddDays(1).AddTicks(-1);
		}
    }
}