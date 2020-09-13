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
    public class NumberInputWindow : Window
    {

        protected SpinButton spinButton;
        protected Label label;
        protected Button okButton;
        protected Button cancelButton;
        protected VBox vBox;
        protected HButtonBox hButtonBox;

        protected NumberInputWindow(string title, string message, int min, int max) : base(WindowType.Toplevel)
        {
            Modal = true;
            Title = title;

            label = new Label(message);
            spinButton = new SpinButton(min, max, 1);
            okButton = new Button("Ok");
            cancelButton = new Button("Cancel");

            vBox = new VBox();

            vBox.Add(label);
            vBox.Add(spinButton);

            hButtonBox = new HButtonBox();

            hButtonBox.Add(cancelButton);
            hButtonBox.Add(okButton);

            vBox.Add(hButtonBox);
            Add(vBox);

        }

        public static void ShowWindow(string title, string message, int min, int max, System.Action<int> action)
        {
            NumberInputWindow numberInputWindow = new NumberInputWindow(title, message, min, max);
            numberInputWindow.okButton.Clicked += (object sender, System.EventArgs e) =>
            {
                action(numberInputWindow.spinButton.ValueAsInt);
                numberInputWindow.Destroy();
            };
            numberInputWindow.cancelButton.Clicked += (object sender, System.EventArgs e) =>
            {
                numberInputWindow.Destroy();
            };
            numberInputWindow.ShowAll();
        }
    }
}
