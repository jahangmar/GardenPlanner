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
        protected SpinButton CYearButton = new SpinButton(2000, 2100, 1);
        protected SpinButton CMonthButton = new SpinButton(1, 12, 1);
        protected SpinButton RYearButton = new SpinButton(2000, 2100, 1);
        protected SpinButton RMonthButton = new SpinButton(1, 12, 1);


        protected GardenAreaCreationDialog(string title) : base(title)
        {
            HBox hbox;

            hbox = new HBox();
            hbox.Add(new Label("Name"));
            hbox.Add(NameEntry);
            EditVBox.Add(hbox);

            hbox = new HBox();
            hbox.Add(new Label("Description"));
            hbox.Add(DescrEntry);
            EditVBox.Add(hbox);

            hbox = new HBox();
            hbox.Add(new Label("Year Created"));
            hbox.Add(CYearButton);
            EditVBox.Add(hbox);

            hbox = new HBox();
            hbox.Add(new Label("Month Created"));
            hbox.Add(CMonthButton);
            EditVBox.Add(hbox);

            hbox = new HBox();
            hbox.Add(new Label("Year Removed"));
            hbox.Add(RYearButton);
            EditVBox.Add(hbox);

            hbox = new HBox();
            hbox.Add(new Label("Month Removed"));
            hbox.Add(RMonthButton);
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

            CYearButton.Value = MainWindow.GetInstance().GetYear();
            RYearButton.Value = MainWindow.GetInstance().GetYear();
            CMonthButton.Value = MainWindow.GetInstance().GetMonth();
            RMonthButton.Value = MainWindow.GetInstance().GetMonth();

            ShowAll();
        }

        static protected void SetValuesForCreation(GardenArea area, List<Garden.GardenPoint> points, GardenAreaCreationDialog dialog)
        {
            area.Shape.AddPoints(points);
            area.Shape.FinishPoints();
            area.SetCreated(dialog.CYearButton.ValueAsInt, dialog.CMonthButton.ValueAsInt);
            area.SetRemoved(dialog.RYearButton.ValueAsInt, dialog.RMonthButton.ValueAsInt);
        }

        public static void ShowGardenAreaCreationDialog(List<GardenPoint> points, System.Action<GardenArea> action)
        {
            GardenAreaCreationDialog dialog = new GardenAreaCreationDialog("Create method area");

            dialog.CreateButton.Clicked += (object sender, System.EventArgs e) =>
            {
                GardenArea area = new Garden.Garden(dialog.NameEntry.Text, dialog.DescrEntry.Text);
                SetValuesForCreation(area, points, dialog);
                action(area);
                GardenDrawingArea.ActiveInstance?.Draw();
                dialog.Destroy();
            };
        }

        public static void ShowGardenAreaEditDialog(GardenArea area)
        {
            string title = "Edit method area '" + area.Name + "'";
            if (area is Garden.Garden)
                title = "Edit garden '" + area.Name + "'";
            else if (area is Planting)
                title = "Edit planting '" + area.Name + "'";
            GardenAreaCreationDialog dialog = new GardenAreaCreationDialog(title);

            dialog.NameEntry.Text = area.Name;
            dialog.DescrEntry.Text = area.Description;
            dialog.CYearButton.Value = area.created.Year;
            dialog.CMonthButton.Value = area.created.Month;
            dialog.RYearButton.Value = area.removed.Year;
            dialog.RMonthButton.Value = area.removed.Month;

            dialog.CreateButton.Clicked += (object sender, System.EventArgs e) =>
            {
                area.Name = dialog.NameEntry.Text;
                area.Description = dialog.DescrEntry.Text;
                area.SetCreated(dialog.CYearButton.ValueAsInt, dialog.CMonthButton.ValueAsInt);
                area.SetRemoved(dialog.RYearButton.ValueAsInt, dialog.RMonthButton.ValueAsInt);
                GardenDrawingArea.ActiveInstance.MakeSelection();
                GardenDrawingArea.ActiveInstance?.Draw();
                dialog.Destroy();
            };
        }
    }
}
