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
    public class GardenAreaCreationDialog : EditWindow
    {
        protected Entry NameEntry = new Entry();
        protected Entry DescrEntry = new Entry();

        protected Button CreateButton = new Button("Create");
        protected Button CancelButton = new Button("Cancel");

        DateEntryBox CreatedDateBox = new DateEntryBox("Date Created");
        DateEntryBox RemovedDateBox = new DateEntryBox("Date Removed");

        protected GardenAreaCreationDialog(string title) : base(title)
        {
            TransientFor = MainWindow.GetInstance();
            Modal = true;

            AddLabeledEntry("Name", NameEntry);
            AddLabeledEntry("Description", DescrEntry);

            AddEntry(CreatedDateBox);
            AddEntry(RemovedDateBox);

            AddControlButton(CancelButton);
            AddControlButton(CreateButton);

            GardenDrawingArea.ActiveInstance?.Draw();

            CancelButton.Clicked += (object sender, System.EventArgs e) =>
            {
                Dialog dialog = new MessageDialog(this, DialogFlags.Modal, MessageType.Warning, ButtonsType.OkCancel, "Are you sure to discard the changes made?", new { });

                int response = dialog.Run();
                if (response == (int)ResponseType.Cancel)
                {

                }
                else if (response == (int)ResponseType.Ok)
                {
                    GardenDrawingArea.ActiveInstance?.NewPoints.Clear();
                    GardenDrawingArea.ActiveInstance?.Draw();
                    this.Destroy();
                }
                dialog.Destroy();

            };

            System.DateTime defaultDate = new System.DateTime(MainWindow.GetInstance().GetYear(), MainWindow.GetInstance().GetMonth(), 1);
            CreatedDateBox.SetDate(defaultDate);
            RemovedDateBox.SetDate(defaultDate);

            ShowAll();
        }

        protected void SetValuesForCreation(GardenArea area, List<Garden.GardenPoint> points)
        {
            area.Shape.AddPoints(points);
            area.Shape.FinishPoints();
            area.SetCreated(CreatedDateBox.GetYear(), CreatedDateBox.GetMonth());
            area.SetRemoved(RemovedDateBox.GetYear(), RemovedDateBox.GetMonth());
            GardenData.unsaved = true;
        }

        public static void ShowGardenAreaCreationDialog(List<GardenPoint> points, System.Action<GardenArea> action)
        {
            GardenAreaCreationDialog dialog = new GardenAreaCreationDialog("Create method area");

            dialog.CreateButton.Clicked += (object sender, System.EventArgs e) =>
            {
                GardenArea area = new Garden.Garden(dialog.NameEntry.Text, dialog.DescrEntry.Text);
                dialog.SetValuesForCreation(area, points);
                action(area);
                GardenDrawingArea.ActiveInstance?.Draw();
                dialog.Destroy();

            };
        }

        protected void SetValuesForEdit(GardenArea area)
        {
            CreateButton.Label = "Save";
            NameEntry.Text = area.Name;
            DescrEntry.Text = area.Description;
            CreatedDateBox.SetDate(area.created);
            RemovedDateBox.SetDate(area.removed);

            CreateButton.Clicked += (object sender, System.EventArgs e) =>
            {
                this.SetValues(area);
                GardenDrawingArea.ActiveInstance.MakeSelection();
                GardenDrawingArea.ActiveInstance?.Draw();
                MainWindow.GetInstance().ShowAreaSelectionInfo(area);
                this.Destroy();
                GardenData.unsaved = true;
            };
        }

        protected virtual void SetValues(GardenArea area)
        {
            area.Name = this.NameEntry.Text;
            area.Description = this.DescrEntry.Text;
            area.SetCreated(CreatedDateBox.GetYear(), CreatedDateBox.GetMonth());
            area.SetRemoved(RemovedDateBox.GetYear(), RemovedDateBox.GetMonth());
        }

        public static void ShowGardenAreaEditDialog(GardenArea area)
        {
            string title = "Edit method area '" + area.Name + "'";
            if (area is Garden.Garden)
                title = "Edit garden '" + area.Name + "'";
            else if (area is Planting)
                title = "Edit planting '" + area.Name + "'";
            GardenAreaCreationDialog dialog = new GardenAreaCreationDialog(title);
            dialog.SetValuesForEdit(area);
        }
    }
}
