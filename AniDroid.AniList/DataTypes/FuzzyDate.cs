using System;
using System.Globalization;

namespace AniDroid.AniList.DataTypes
{
    public class FuzzyDate
    {
        public int? Year { get; set; }
        public int? Month { get; set; }
        public int? Day { get; set; }

        public FuzzyDate()
        {
        }

        public FuzzyDate(DateTime? date)
        {
            Year = date?.Year ?? 0;
            Month = date?.Month ?? 0;
            Day = date?.Day ?? 0;
        }

        public DateTime? GetDate()
        {
            if (Year > 0 && Month > 0 && Day > 0)
            {
                try
                {
                    return new DateTime(Year.Value, Month.Value, Day.Value);
                }
                catch
                {
                    return null;
                }
            }

            return null;
        }

        public DateTime GetFuzzyDate()
        {
            return new DateTime(Year ?? DateTime.Now.Year, Month ?? 1, Day ?? 1);
        }

        public bool IsValid()
        {
            return Year > 0 || Month > 0;
        }

        public bool Equals(FuzzyDate date)
        {
            var equal = true;

            if (Year.HasValue && date.Year.HasValue)
            {
                equal = Year.Value == date.Year.Value;
            }

            if (Month.HasValue && date.Month.HasValue)
            {
                equal = Month.Value == date.Month.Value;
            }

            if (Day.HasValue && date.Day.HasValue)
            {
                equal = Day.Value == date.Day.Value;
            }

            return equal;
        }

        public string GetFuzzyDateString()
        {
            if (!(Month > 0 && Month < 13))
            {
                return Year?.ToString();
            }

            var retString = DateTimeFormatInfo.CurrentInfo?.GetMonthName(Month.Value);
            retString += Day.HasValue ? $" {Day.Value}" : "";
            retString += Year.HasValue ? $", {Year.Value}" : "";
            return retString;
        }
    }
}
