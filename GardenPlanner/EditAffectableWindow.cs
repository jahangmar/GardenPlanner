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

using GardenPlanner.Garden;

namespace GardenPlanner
{
    public abstract class EditAffectableWindow<T> : Window
    {
        VPaned VPaned = new VPaned();

        VBox EntryBox = new VBox();

        protected Entry NameEntry;
        protected TextView DescriptionTextView;
        protected Entry SciNameEntry;

        HButtonBox ActionButtonBox = new HButtonBox();
        Button SaveButton = new Button(new Label("Save"));
        Button InfoButton = new Button(new Label("Info"));
        Button DeleteButton = new Button(new Label("Delete"));
        Button DiscardButton = new Button(new Label("Discard"));

        public EditAffectableWindow(string title, Affectable affectable) : base(WindowType.Toplevel)
        {
            Modal = true;

            Title = title;

            TransientFor = MainWindow.GetInstance();

            VPaned.Add(EntryBox);

            NameEntry = new Entry(affectable.Name);
            AddEntry("Name", NameEntry);
            SciNameEntry = new Entry(affectable.ScientificName);
            AddEntry("Scientific Name", SciNameEntry);
            DescriptionTextView = new TextView();
            DescriptionTextView.Buffer.Text = affectable.Description;
            AddEntry("Description ", DescriptionTextView);


            ActionButtonBox.Add(InfoButton);
            ActionButtonBox.Add(SaveButton);
            ActionButtonBox.Add(DeleteButton);
            ActionButtonBox.Add(DiscardButton);
            VPaned.Add2(ActionButtonBox);

            this.Add(VPaned);
            InfoButton.Clicked += (object sender, System.EventArgs e) => Info();
            DiscardButton.Clicked += (object sender, System.EventArgs e) => TryToClose();
            SaveButton.Clicked += (object sender, System.EventArgs e) => Save();
            DeleteButton.Clicked += (object sender, System.EventArgs e) => Delete();
        }

        private void TryToClose()
        {
            Dialog dialog = new MessageDialog(this, DialogFlags.Modal, MessageType.Warning, ButtonsType.OkCancel, "Are you sure to discard the changes made?", new { });

            int response = dialog.Run();
            //System.Console.WriteLine("response " + result);
            if (response == (int)ResponseType.Cancel)
            {

            }
            else if (response == (int)ResponseType.Ok)
            {
                this.Destroy();
            }
            dialog.Destroy();
        }

        public void AddEntry(Widget widget)
        {
            EntryBox.Add(widget);
        }

        public void AddEntry(string label, Widget widget)
        {
            HBox hBox = new HBox();
            hBox.Add(new Label(label));
            hBox.Add(widget);
            EntryBox.Add(hBox);
        }

        public void ShowSuccessSave(string name)
        {
            Dialog dialog = new MessageDialog(this, DialogFlags.Modal, MessageType.Info, ButtonsType.Ok, "Successfully saved " + name, new { });
            dialog.Run();
            dialog.Destroy();
            this.Destroy();
            MainWindow.GetInstance().RepopulateGrowables();
            GardenData.unsaved = true;
        }

        public void ShowErrorSave(string name, string reason = "")
        {
            Dialog dialog = new MessageDialog(this, DialogFlags.Modal, MessageType.Error, ButtonsType.Close, "Error while saving"+ (reason.Length > 0 ? ": "+reason : ""), new { });
            dialog.Run();
            dialog.Destroy();
        }

        protected Affectable ModifyOrCreate(Affectable affectable = null)
        {
            affectable.Name = NameEntry.Text;
            affectable.Description = DescriptionTextView.Buffer.Text;
            affectable.ScientificName = SciNameEntry.Text;
            return affectable;
        }

        protected abstract void Info();
        protected abstract void Save();
        protected virtual void Delete()
        {
            MainWindow.GetInstance().ReloadFamilies();
            GardenDrawingArea.ActiveInstance?.Draw();
            this.Destroy();
        }

        protected bool DeleteDialog(System.Action action, string name)
        {
            Dialog dialog = new MessageDialog(this, DialogFlags.Modal, MessageType.Warning, ButtonsType.OkCancel, "Do you want to delete '" + name + "'?", new { });

            int response = dialog.Run();
            if (response == (int)ResponseType.Cancel)
            {
                dialog.Destroy();
                return false;
            }
            else if (response == (int)ResponseType.Ok)
            {
                action();
                dialog.Destroy();
                return true;
            }
            return false;
        }
    }
}
