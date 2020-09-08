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
