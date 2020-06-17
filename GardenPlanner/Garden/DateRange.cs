using System;
namespace GardenPlanner.Garden
{
    public class DateRange
    {
        private const int defyear = 2000;

        public DateRange()
        {
            start = DateTime.Parse("01.01." + defyear);
            end = DateTime.Parse("01.01." + defyear);
        }

        private DateTime start;
        private DateTime end;

        public void SetStart(string s)
        {
            start = DateTime.Parse(s);
            if (start.Year == defyear)
                start.AddYears(1);
        }

        public void SetEnd(string s)
        {
            end = DateTime.Parse(s);
            if (end.Year == defyear)
                end.AddYears(1);
        }

        public int GetDaysUntilHarvest() => 0;

        public override string ToString()
        {
            Translation trans = Translation.GetTranslation();
            return trans.From + " " + DateTimeToString(start) + " " + trans.To + " " + DateTimeToString(end);
        }

        private string DateTimeToString(DateTime dateTime)
        {
            Translation trans = Translation.GetTranslation();
            if (dateTime.Year == defyear)
            {
                return trans.Unset;
            }
            return dateTime.Day + "." + dateTime.Month;
        }
    }
}
