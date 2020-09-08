using System;
using System.Collections.Generic;
using GardenPlanner.Garden;

namespace GardenPlanner
{
    public class PlantingCreationDialog : GardenAreaCreationDialog
    {
        protected PlantingCreationDialog() : base("Create new Planting")
        {

        }


        public static void ShowPlantingCreationDialog(List<GardenPoint> points, Action<Planting> action)
        {
            PlantingCreationDialog dialog = new PlantingCreationDialog();

            dialog.CreateButton.Clicked += (object sender, System.EventArgs e) =>
            {
                Planting area = new Planting(dialog.NameEntry.Text, dialog.DescrEntry.Text);
                SetValuesForCreation(area, points, dialog);
                action(area);
                GardenDrawingArea.ActiveInstance?.Draw();
                dialog.Destroy();
            };
        }
    }
}
