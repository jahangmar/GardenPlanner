using System;
using System.Collections.Generic;
using GardenPlanner.Garden;

namespace GardenPlanner
{
    public class GardenCreationDialog : GardenAreaCreationDialog
    {
        protected GardenCreationDialog(List<GardenPoint> points) : base("Create new garden", points)
        {

        }

        public static void ShowGardenCreationDialog(List<GardenPoint> points, Action<Garden.Garden> action)
        {
            GardenCreationDialog dialog = new GardenCreationDialog(points);

            dialog.CreateButton.Clicked += (object sender, System.EventArgs e) =>
            {
                Garden.Garden garden = new Garden.Garden(dialog.NameEntry.Text, dialog.DescrEntry.Text);
                garden.Shape.AddPoints(points);
                garden.Shape.FinishPoints();
                //TODO
                action(garden);
            };
        }
    }
}
