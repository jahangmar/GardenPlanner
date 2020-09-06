using System;
using System.Collections.Generic;
using GardenPlanner.Garden;

namespace GardenPlanner
{
    public class PlantingCreationDialog : GardenAreaCreationDialog
    {
        protected PlantingCreationDialog(List<GardenPoint> points) : base("Create new Planting", points)
        {

        }


        public static void ShowPlantingCreationDialog(List<GardenPoint> points, Action<Planting> action)
        {
            PlantingCreationDialog dialog = new PlantingCreationDialog(points);

            dialog.CreateButton.Clicked += (object sender, System.EventArgs e) =>
            {
                Planting area = new Planting(dialog.NameEntry.Text, dialog.DescrEntry.Text);
                SetValues(area, points, dialog);

                action(area);
                dialog.Destroy();
            };
        }
    }
}
