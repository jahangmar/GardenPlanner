// Copyright (c) 2020 Jahangmar
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <https://www.gnu.org/licenses/>.
//

using System;
namespace GardenPlanner.Garden
{
    public class DateRange
    {
        private const int defyear = 1900;

        private DateTime start;
        private DateTime end;

        public DateRange()
        {
            start = DateTime.Parse("01.01." + defyear);
            end = DateTime.Parse("01.01." + defyear);
        }

        public DateRange(int startyear, int startmonth, int startday, int endyear, int endmonth, int endday)
        {
            start = new DateTime(startyear, startmonth, startday);
            end = new DateTime(endyear, endmonth, endday);
            CheckEnd();
        }

        public DateRange(int startyear, int startmonth, int endyear, int endmonth) : this(startyear, startmonth, 1, endyear, endmonth, 1)
        {

        }

        public DateTime GetStart() => start;
        public DateTime GetEnd() => end;

        public int GetRangeInDays() => (end - start).Days;
        public int GetRangeInRoundMonths() => (int) System.Math.Round(GetRangeInMonth());
        public float GetRangeInMonth() => GetRangeInDays() / 30f;
        public int GetRangeInRoundYears() => (int)System.Math.Round(GetRangeInYears());
        public float GetRangeInYears() => GetRangeInDays() / 365f;

        private bool IsDefault(DateTime dateTime) => dateTime.Year == defyear;
        public bool IsDefault() => IsDefault(start) || IsDefault(end);

        public bool IsDateInRange(int year, int month) => (year == start.Year && month >= start.Month || year > start.Year) &&
            (year == end.Year && month <= end.Month || year < end.Year) || year == 0 && month == 0;

        /// <summary>
        /// Checks if the given date is in this range. Ignores the day.
        /// </summary>
        public bool IsDateInRange(DateTime dateTime) => IsDateInRange(dateTime.Year, dateTime.Month);

        private void CheckStart()
        {
            if (start > end && !IsDefault(end))
                start = end;
        }

        private void CheckEnd()
        {
            if (end < start && !IsDefault(start))
                end = start;
        }

        public void SetStartDay(int day)
        {
            start = new DateTime(start.Year, start.Month, day);
            CheckStart();
        }

        public void SetStartMonth(int month)
        {
            start = new DateTime(start.Year, month, start.Day);
            CheckStart();
        }

        public void SetStartYear(int year)
        {
            start = new DateTime(year, start.Month, start.Day);
            CheckStart();
        }

        public void SetStartYearMonth(int year, int month)
        {
            start = new DateTime(year, month, 1);
            CheckStart();
        }

        public void SetStartYearMonthDay(int year, int month, int day)
        {
            start = new DateTime(year, month, day);
            CheckStart();
        }

        public void SetStart(int year, int month, int day) => SetStartYearMonthDay(year, month, day);

        public void SetStart(DateTime dateTime)
        {
            SetStartYearMonthDay(dateTime.Year, dateTime.Month, dateTime.Day);
        }

        public void SetEndDay(int day)
        {
            end = new DateTime(end.Year, end.Month, day);
            CheckEnd();
        }

        public void SetEndMonth(int month)
        {
            end = new DateTime(end.Year, month, end.Day);
            CheckEnd();
        }

        public void SetEndYear(int year)
        {
            end = new DateTime(year, end.Month, end.Day);
            CheckEnd();
        }

        public void SetEndYearMonth(int year, int month)
        {
            end = new DateTime(year, month, 1);
            CheckEnd();
        }

        public void SetEndYearMonthDay(int year, int month, int day)
        {
            end = new DateTime(year, month, day);
            CheckEnd();
        }

        public void SetEnd(int year, int month, int day) => SetEndYearMonthDay(year, month, day);

        public void SetEnd(DateTime dateTime)
        {
            SetEndYearMonthDay(dateTime.Year, dateTime.Month, dateTime.Day);
        }

        public int GetDaysUntilHarvest() => (end - start).Days;

        public bool IsEmpty() => start.Equals(end);

        public override string ToString()
        {
            Translation trans = Translation.GetTranslation();
            return trans.From + " " + DayMonthDateTimeToString(start) + " " + trans.To + " " + DayMonthDateTimeToString(end);
        }

        public string ToFullString() =>
            DayMonthYearDateTimeToString(start) + " - " + DayMonthYearDateTimeToString(end);

        public string DayMonthYearDateTimeToString(DateTime dateTime) =>
            $"{dateTime.Day}.{dateTime.Month}.{dateTime.Year}";

        public static string DayMonthDateTimeToString(DateTime dateTime)
        {
            Translation trans = Translation.GetTranslation();
            if (dateTime.Year == defyear)
            {
                return trans.Unset;
            }
            return dateTime.Day + "." + dateTime.Month + ".";
        }

        public static string ApproxDayMonthDateTimeToString(DateTime dateTime)
        {
            Translation trans = Translation.GetTranslation();
            string s = "";
            if (dateTime.Day <= 10)
            {
                s = trans.Dates_Beginning.Upper0();
            }
            else if (dateTime.Day <= 20)
            {
                s = trans.Dates_Middle.Upper0();
            }
            else
            {
                s = trans.Dates_End.Upper0();
            }
            return s + " " + trans.Dates_Months[dateTime.Month];
        }
    }
}
