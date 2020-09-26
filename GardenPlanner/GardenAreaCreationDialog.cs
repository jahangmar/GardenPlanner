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
using System.Collections.Generic;
using GardenPlanner.Garden;
namespace GardenPlanner
{
    public class GardenAreaCreationDialog : Window
    {
        VBox TopVBox = new VBox();
        protected VBox EditVBox = new VBox();
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
            TransientFor = MainWindow.GetInstance();
            Modal = true;

            AddEditEntry("Name", NameEntry);
            AddEditEntry("Description", DescrEntry);
            AddEditEntry("Year Created", CYearButton);
            AddEditEntry("Month Created", CMonthButton);
            AddEditEntry("Year Removed", RYearButton);
            AddEditEntry("Month Removed", RMonthButton);

            TopVBox.Add(EditVBox);
            ButtonBox.Add(CancelButton);
            ButtonBox.Add(CreateButton);
            TopVBox.Add(ButtonBox);
            this.Add(TopVBox);

            GardenDrawingArea.ActiveInstance?.Draw();

            CancelButton.Clicked += (object sender, System.EventArgs e) =>
            {
                GardenDrawingArea.ActiveInstance?.NewPoints.Clear();
                GardenDrawingArea.ActiveInstance?.Draw();
                this.Destroy();
            };

            CYearButton.Value = MainWindow.GetInstance().GetYear();
            RYearButton.Value = MainWindow.GetInstance().GetYear();
            CMonthButton.Value = MainWindow.GetInstance().GetMonth();
            RMonthButton.Value = MainWindow.GetInstance().GetMonth();

            ShowAll();
        }

        protected HBox AddEditEntry(string label, Widget widget)
        {
            HBox hbox = new HBox();
            hbox.Add(new Label(label));
            hbox.Add(widget);
            EditVBox.Add(hbox);
            return hbox;
        }

        static protected void SetValuesForCreation(GardenArea area, List<Garden.GardenPoint> points, GardenAreaCreationDialog dialog)
        {
            area.Shape.AddPoints(points);
            area.Shape.FinishPoints();
            area.SetCreated(dialog.CYearButton.ValueAsInt, dialog.CMonthButton.ValueAsInt);
            area.SetRemoved(dialog.RYearButton.ValueAsInt, dialog.RMonthButton.ValueAsInt);
            GardenData.unsaved = true;
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

        static protected void SetValuesForEdit(GardenArea area, GardenAreaCreationDialog dialog)
        {
            dialog.CreateButton.Label = "Edit";
            dialog.NameEntry.Text = area.Name;
            dialog.DescrEntry.Text = area.Description;
            dialog.CYearButton.Value = area.created.Year;
            dialog.CMonthButton.Value = area.created.Month;
            dialog.RYearButton.Value = area.removed.Year;
            dialog.RMonthButton.Value = area.removed.Month;

            dialog.CreateButton.Clicked += (object sender, System.EventArgs e) =>
            {
                dialog.SetValues(area);
                GardenDrawingArea.ActiveInstance.MakeSelection();
                GardenDrawingArea.ActiveInstance?.Draw();
                MainWindow.GetInstance().ShowAreaSelectionInfo(area);
                dialog.Destroy();
                GardenData.unsaved = true;
            };
        }

        protected virtual void SetValues(GardenArea area)
        {
            area.Name = this.NameEntry.Text;
            area.Description = this.DescrEntry.Text;
            area.SetCreated(this.CYearButton.ValueAsInt, this.CMonthButton.ValueAsInt);
            area.SetRemoved(this.RYearButton.ValueAsInt, this.RMonthButton.ValueAsInt);
        }

        public static void ShowGardenAreaEditDialog(GardenArea area)
        {
            string title = "Edit method area '" + area.Name + "'";
            if (area is Garden.Garden)
                title = "Edit garden '" + area.Name + "'";
            else if (area is Planting)
                title = "Edit planting '" + area.Name + "'";
            GardenAreaCreationDialog dialog = new GardenAreaCreationDialog(title);
            SetValuesForEdit(area, dialog);
        }
    }
}
