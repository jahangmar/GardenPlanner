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
using Gtk;
namespace GardenPlanner
{
    public enum DateEntryType
    {
        MonthDateEntry,
        MiddleMonthDateEntry,
        DayDateEntry
    }

    public class DateEntryBox : Frame
    {
        SpinButton spinButtonYear;
        SpinButton spinButtonMonth;
        RadioButton radioMiddle;
        RadioButton radioBeginning;
        SpinButton spinButtonDay;

        public DateEntryBox(string label, DateEntryType dateEntryType = DateEntryType.MonthDateEntry) : base(label)
        {
            GardenPlannerSettings settings = GardenPlannerSettings.GetSettings();

            VBox mainVBox = new VBox();

            Label labelYear = new Label("Year");
            spinButtonYear = new SpinButton(settings.MinYear, settings.MaxYear, 1);
            HBox hBoxYear = new HBox
            {
                labelYear,
                spinButtonYear
            };

            mainVBox.Add(hBoxYear);

            Label labelDay = new Label("Day");
            spinButtonDay = new SpinButton(1, 31, 1);
            HBox hBoxDay = new HBox
            {
                labelDay,
                spinButtonDay
            };



            Label labelMonth = new Label("Month");
            spinButtonMonth = new SpinButton(1, 12, 1);
            HBox hBoxMonth = new HBox
            {
                labelMonth,
                spinButtonMonth
            };

            mainVBox.Add(hBoxMonth);

            radioMiddle = new RadioButton("middle");
            radioBeginning = new RadioButton(radioMiddle, "beginning");
            switch (dateEntryType)
            {
                case DateEntryType.MonthDateEntry:
                    break;
                case DateEntryType.MiddleMonthDateEntry:
                    Label labelMiddleMonth = new Label("Time of month");
                    HBox hBoxRadios = new HBox
                    {
                        radioMiddle,
                        radioBeginning
                    };
                    HBox hBoxMiddleMonth = new HBox
                    {
                        labelMiddleMonth,
                        hBoxRadios
                    };

                    mainVBox.Add(hBoxMiddleMonth);
                    break;
                case DateEntryType.DayDateEntry:
                    mainVBox.Add(hBoxDay);
                    break;
            }

            this.Add(mainVBox);
        }

        public DateEntryBox(string label, DateTime date, DateEntryType dateEntryType = DateEntryType.MonthDateEntry) : this(label, dateEntryType)
        {
            SetDate(date);
        }

        public void SetDate(DateTime date)
        {
            spinButtonYear.Value = date.Year;
            spinButtonMonth.Value = date.Month;
            radioBeginning.Active = (date.Day < 15);
            spinButtonDay.Value = date.Day;
        }

        public DateTime GetDate()
        {
            return new DateTime(GetYear(), GetMonth(), GetDay());
        }

        public int GetYear() => spinButtonYear.ValueAsInt;
        public int GetMonth() => spinButtonMonth.ValueAsInt;
        public int GetDay() => radioBeginning.Active ? 1 : 15;
    }
}
