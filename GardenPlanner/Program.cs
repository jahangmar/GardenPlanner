using System;
using Gtk;

using GardenPlanner.Garden;

namespace GardenPlanner
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            Application.Init();
            MainWindow win = MainWindow.GetInstance();
            win.Show();
            Application.Run();
        }

        public static GardenData TestData()
        {
            GardenData Data = new GardenData();

            var zwiebelgewaechse = new PlantFamily("Zwiebelgewaechse", "");
            Data.AddFamily("zwiebelgewaechse", zwiebelgewaechse);
            var knoblauch = new Plant("Knoblauch", "");
            knoblauch.FeederType = FeederType.Medium;
            zwiebelgewaechse.AddPlant("knoblauch", knoblauch);
            var morado = new PlantVariety("Morado", "rotviolett, aus Spanien, geeignet für Herbstpflanzung, bildet Brutzwiebeln");
            knoblauch.AddVariety("morado", morado);

            var nachtschattengew = new PlantFamily("Nachtschattengewächse", "");
            Data.AddFamily("nachtschattengewaechse", nachtschattengew);
            var kartoffeln = new Plant("Kartoffel", "");
            nachtschattengew.AddPlant("kartoffel", kartoffeln);

            var bed1 = new Garden.Garden("Omas Garten", "Alte Garten von Oma");
            var bed2 = new Garden.Garden("Hauptstraßengarten", "Alte Garten von Oma");
            Data.AddGarden("oma_garten", bed1);
            Data.AddGarden("hptstr_garten", bed2);
            bed1.Shape.AddPoint(new GardenPoint(0, 0));
            bed1.Shape.AddPoint(new GardenPoint(0, 200));
            bed1.Shape.AddPoint(new GardenPoint(300, 200));
            bed1.Shape.AddPoint(new GardenPoint(300, 0));
            bed1.Shape.AddPoint(new GardenPoint(0, 0));

            string s = Newtonsoft.Json.JsonConvert.SerializeObject(Data, Newtonsoft.Json.Formatting.Indented);

            return Data;
        }
    }
}

/*TODO
 * -set growable values for varieties when adding it to plant based on values of plant
 * -same as above for plant, family, and Affectable interace
 * 
 * 
 * 
 * 
 */ 