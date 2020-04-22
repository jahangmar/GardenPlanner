using Gtk;

using GardenPlanner.Garden;

namespace GardenPlanner
{
    public abstract class EditAffectableWindow<T> : Window
    {
        VPaned VPaned = new VPaned();

        VBox EntryBox = new VBox();

        HButtonBox ActionButtonBox = new HButtonBox();
        Button SaveButton = new Button(new Label("Save"));
        Button DeleteButton = new Button(new Label("Delete"));
        Button DiscardButton = new Button(new Label("Discard"));

        public EditAffectableWindow(string title) : base(WindowType.Toplevel)
        {
            Modal = true;

            Title = title;

            VPaned.Add(EntryBox);

            ActionButtonBox.Add(SaveButton);
            ActionButtonBox.Add(DeleteButton);
            ActionButtonBox.Add(DiscardButton);
            VPaned.Add2(ActionButtonBox);

            this.Add(VPaned);

            DiscardButton.Clicked += (object sender, System.EventArgs e) => TryToClose();
            SaveButton.Clicked += (object sender, System.EventArgs e) => Save();
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
        }

        public void ShowErrorSave(string name, string reason = "")
        {
            Dialog dialog = new MessageDialog(this, DialogFlags.Modal, MessageType.Error, ButtonsType.Close, "Error while saving"+ (reason.Length > 0 ? ": "+reason : ""), new { });
            dialog.Run();
            dialog.Destroy();
        }

        public abstract void Save();
        public abstract void Delete(T affectable);
    }
}
