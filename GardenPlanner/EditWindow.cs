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
    public abstract class EditWindow : Window
    {
        protected VPaned mainVPaned;
        protected VBox entryVBox;
        protected HBox controlHBox;

        public EditWindow(string title) : base(WindowType.Toplevel)
        {
            Modal = true;
            Title = title;

            mainVPaned = new VPaned();
            entryVBox = new VBox();
            controlHBox = new HBox();

            mainVPaned.Add(entryVBox);
            mainVPaned.Add(controlHBox);
            Add(mainVPaned);
        }

        protected void AddControlButton(Button button)
        {
            controlHBox.Add(button);
        }

        protected void AddControlButton(string label, System.Action action)
        {
            Button button = new Button(label);
            button.Clicked += (sender, e) => action.Invoke();
            AddControlButton(button);
        }

        protected HBox AddLabeledEntry(string label, Widget widget)
        {
            HBox hBox = new HBox()
            {
                new Label(label),
                widget
            };
            entryVBox.Add(hBox);
            return hBox;
        }

        protected void RemoveLabeledEntry(HBox hBox)
        {
            RemoveEntry(hBox);
        }

        protected void RemoveEntry(Widget widget)
        {
            entryVBox.Remove(widget);
        }

        protected void AddEntry(Widget widget)
        {
            entryVBox.Add(widget);
        }
    }
}
