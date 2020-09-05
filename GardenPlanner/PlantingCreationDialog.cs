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
                System.Console.WriteLine("create");
                Planting planting = new Planting(dialog.NameEntry.Text, dialog.DescrEntry.Text);
                planting.Shape.AddPoints(points);
                planting.Shape.FinishPoints();
                planting.SetCreated(2000, 1);
                planting.SetRemoved(2010, 1);
                //TODO
                points.ForEach((GardenPoint obj) => System.Console.WriteLine("point " + obj.X + "/" + obj.Y));

                action(planting);
                dialog.Destroy();
            };
        }
    }
}
