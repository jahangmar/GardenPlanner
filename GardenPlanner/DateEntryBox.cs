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
    public class DateEntryBox : Frame
    {
        SpinButton spinButtonYear;
        SpinButton spinButtonMonth;
        RadioButton radioMiddle;
        RadioButton radioBeginning;

        public DateEntryBox(string label, bool showMiddleButton = false) : base(label)
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
            if (showMiddleButton)
            {
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
            }

            this.Add(mainVBox);
        }

        public DateEntryBox(string label, DateTime date, bool showMiddleButton = false) : this(label, showMiddleButton)
        {
            SetDate(date);
        }

        public void SetDate(DateTime date)
        {
            spinButtonYear.Value = date.Year;
            spinButtonMonth.Value = date.Month;
            radioBeginning.Active = (date.Day < 15);
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
