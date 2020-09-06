using System;
using System.Collections.Generic;
using GardenPlanner.Garden;

namespace GardenPlanner
{
    public class GardenCreationDialog : GardenAreaCreationDialog
    {
        protected GardenCreationDialog() : base("Create new garden")
        {

        }

        public static void ShowGardenCreationDialog(List<GardenPoint> points, Action<Garden.Garden> action)
        {
            GardenCreationDialog dialog = new GardenCreationDialog();

            dialog.CreateButton.Clicked += (object sender, System.EventArgs e) =>
            {
                Garden.Garden area = new Garden.Garden(dialog.NameEntry.Text, dialog.DescrEntry.Text);
                SetValuesForCreation(area, points, dialog);
                action(area);
                GardenDrawingArea.ActiveInstance?.Draw();
                dialog.Destroy();
            };
        }
    }
}
