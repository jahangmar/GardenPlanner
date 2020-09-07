using System;
using Gtk;

using GardenPlanner.Garden;

namespace GardenPlanner
{
    class MainClass
    {

        public static string MAIN_PATH = "/home/jan/.gardenplanner/";

        public static void Main(string[] args)
        {
            Translation translation = new Translation();

            //System.IO.StreamWriter streamWriter = new System.IO.StreamWriter("/tmp/gplanner.json");
            //string ser = Newtonsoft.Json.JsonConvert.SerializeObject(translation);
            //System.Console.WriteLine(ser);
            //streamWriter.Write(ser);
            //streamWriter.Flush();

            Application.Init();
            MainWindow win = MainWindow.GetInstance();
            win.Show();
            Application.Run();
        }

        public static GardenData TestData()
        {
            GardenData Data = new GardenData("testdata");

            var zwiebelgewaechse = new PlantFamily("Zwiebelgewaechse", "");
            Data.AddFamily("zwiebelgewaechse", zwiebelgewaechse);
            var knoblauch = new Plant("Knoblauch", "");
            knoblauch.FeederType = FeederType.Medium;
            knoblauch.ScientificName = "allium sativum";
            zwiebelgewaechse.AddPlant("knoblauch", knoblauch);
            var morado = new PlantVariety("Morado", "rotviolett, aus Spanien, geeignet für Herbstpflanzung, bildet Brutzwiebeln");
            morado.Color = new Cairo.Color(0.9, 0.9, 0.9);
            knoblauch.AddVariety("morado", morado);
            var vallelado = new PlantVariety("Vallelado", "bla");
            vallelado.Color = new Cairo.Color(0.9, 0.9, 0.9);
            knoblauch.AddVariety("vallelado", vallelado);

            var nachtschattengew = new PlantFamily("Nachtschattengewächse", "");
            Data.AddFamily("nachtschattengewaechse", nachtschattengew);
            var kartoffeln = new Plant("Kartoffel", "");
            kartoffeln.Color = new Cairo.Color(0.2, 0.8, 0.8);
            nachtschattengew.AddPlant("kartoffel", kartoffeln);

            var bed1 = new Garden.Garden("Omas Garten", "Alte Garten von Oma", 2000, 1, 2000,1);
            var bed2 = new Garden.Garden("Hauptstraßengarten", "Alte Garten von Oma", 2000, 1, 2000, 1);
            Data.AddGarden("oma_garten", bed1);
            Data.AddGarden("hptstr_garten", bed2);
            bed1.Shape.AddPoint(new GardenPoint(0, 0));
            bed1.Shape.AddPoint(new GardenPoint(0, 200));
            bed1.Shape.AddPoint(new GardenPoint(300, 200));
            bed1.Shape.AddPoint(new GardenPoint(300, 0));
            bed1.Shape.FinishPoints();
            var compostArea = new GardenArea("Kompost", "hier wurde Kompost angewendet", 2000, 1, 2010, 6);
            compostArea.Shape.AddPoint(new GardenPoint(100, 100));
            compostArea.Shape.AddPoint(new GardenPoint(300, 100));
            compostArea.Shape.AddPoint(new GardenPoint(300, 200));
            compostArea.Shape.AddPoint(new GardenPoint(100, 200));
            compostArea.Shape.FinishPoints();
            bed1.AddMethodArea("compost", compostArea);
            var plantingArea = new Planting("Planting", "hier wurde was gepflanzt", 2000, 1, 2010, 6);
            plantingArea.Shape.AddPoint(new GardenPoint(400, 400));
            plantingArea.Shape.AddPoint(new GardenPoint(500, 400));
            plantingArea.Shape.AddPoint(new GardenPoint(500, 500));
            plantingArea.Shape.AddPoint(new GardenPoint(400, 500));
            plantingArea.Shape.FinishPoints();
            bed1.AddPlanting("planting", plantingArea);
            plantingArea.AddVariety(morado, 3);
            plantingArea.AddVariety(vallelado, 2);

            //string s = Newtonsoft.Json.JsonConvert.SerializeObject(Data, Newtonsoft.Json.Formatting.Indented);

            return Data;
        }
    }
}

/*TODO
 * -scrolling
 * -add possibility to add images (automatically)
 * -maybe add references (e.g. varieities have plant reference "GetPlant()" that only checks once per Load)
 */
