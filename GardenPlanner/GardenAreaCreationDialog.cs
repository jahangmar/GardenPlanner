using Gtk;
using System.Collections.Generic;
using GardenPlanner.Garden;
namespace GardenPlanner
{
    public class GardenAreaCreationDialog : Window
    {
        VBox TopVBox = new VBox();
        VBox EditVBox = new VBox();
        protected Entry NameEntry = new Entry();
        protected Entry DescrEntry = new Entry();

        HButtonBox ButtonBox = new HButtonBox();
        protected Button CreateButton = new Button("Create");
        protected Button CancelButton = new Button("Cancel");

        protected List<GardenPoint> Points;

        protected GardenAreaCreationDialog(string title, List<GardenPoint> points) : base(title)
        {
            Points = points;

            HBox hbox;
            hbox = new HBox();
            hbox.Add(new Label("Name"));
            hbox.Add(NameEntry);
            EditVBox.Add(hbox);

            TopVBox.Add(EditVBox);
            ButtonBox.Add(CancelButton);
            ButtonBox.Add(CreateButton);
            TopVBox.Add(ButtonBox);
            this.Add(TopVBox);

            CancelButton.Clicked += (object sender, System.EventArgs e) =>
            {
                this.Destroy();
            };

            ShowAll();
        }

        public static void ShowGardenAreaCreationDialog(List<GardenPoint> points, System.Action<GardenArea> action)
        {
            GardenAreaCreationDialog dialog = new GardenAreaCreationDialog("Create method area", points);

            dialog.CreateButton.Clicked += (object sender, System.EventArgs e) =>
            {
                GardenArea area = new Garden.Garden(dialog.NameEntry.Text, dialog.DescrEntry.Text);
                area.Shape.AddPoints(points);
                area.Shape.FinishPoints();
                //TODO
                action(area);
            };
        }
    }
}
