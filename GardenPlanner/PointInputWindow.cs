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
using Gtk;
namespace GardenPlanner
{
    class PointInputWindow : NumberInputWindow
    {
        Label label2;
        SpinButton spinButton2;

        private PointInputWindow(string title, int min, int max, int x, int y) : base(title, "X", min, max)
        {
            label2 = new Label("Y");
            spinButton2 = new SpinButton(min, max, 1);
            vBox.Remove(hButtonBox);
            vBox.Add(label2);
            vBox.Add(spinButton2);
            vBox.Add(hButtonBox);
            spinButton.Value = x;
            spinButton2.Value = y;
        }

        public static void ShowWindow(string title, int min, int max, int x, int y, System.Action<int, int> action)
        {
            PointInputWindow pointInputWindow = new PointInputWindow(title, min, max, x, y);
            pointInputWindow.okButton.Clicked += (object sender, System.EventArgs e) =>
            {
                action(pointInputWindow.spinButton.ValueAsInt, pointInputWindow.spinButton2.ValueAsInt);
                pointInputWindow.Destroy();
            };
            pointInputWindow.cancelButton.Clicked += (object sender, System.EventArgs e) =>
            {
                pointInputWindow.Destroy();
            };
            pointInputWindow.ShowAll();
        }
    }
}
