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

using System.Collections.Generic;
using Gtk;

namespace GardenPlanner
{
    public abstract class InfoAffectableWindow : Window
    {
        VPaned VPaned = new VPaned();

        protected HButtonBox ActionButtonBox = new HButtonBox();
        Button CloseButton = new Button(new Label("Close"));
        Button EditButton = new Button(new Label("Edit"));

        protected InfoView infoView = new InfoView();

        public InfoAffectableWindow(string title, bool isEdited = false) : base(WindowType.Toplevel)
        {
            Modal = true;
            Title = title;

            TransientFor = MainWindow.GetInstance();

            VPaned.Add1(infoView);
            VPaned.Add2(ActionButtonBox);

            this.Add(VPaned);

            ActionButtonBox.Add(CloseButton);
            ActionButtonBox.Add(EditButton);

            CloseButton.Clicked += (object sender, System.EventArgs e) => this.Destroy();
            EditButton.Clicked += (object sender, System.EventArgs e) =>
            {
                if (isEdited)
                    this.Destroy();
                else
                    Edit();
            };

            this.Destroyed += (object o, System.EventArgs args) =>
            {
                //MainWindow.GetInstance().Sensitive = true;
            };

            //MainWindow.GetInstance().Sensitive = false;
        }
              
        protected int AddEntry(string entry, bool newline=true) => infoView.AddEntry(entry, newline);

        //TextIter iter;
        protected void AddEntry(string entry, TextTag tag, bool newline = true) => infoView.AddEntry(entry, tag, newline);

        protected void ApplyTags() => infoView.ApplyTags();

        protected abstract void Edit();
    }
}
