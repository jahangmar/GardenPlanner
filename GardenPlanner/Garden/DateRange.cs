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
