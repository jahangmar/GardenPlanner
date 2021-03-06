﻿// Copyright (c) 2020 Jahangmar
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

using System;
using System.IO;
using Gtk;

using GardenPlanner.Garden;

namespace GardenPlanner
{
    class MainClass
    {
        public static string MAIN_PATH = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "gardenplanner");

        public static string VERSION = "0.0.0-alpha";

        public static void Main(string[] args)
        {
            //System.IO.StreamWriter streamWriter = new System.IO.StreamWriter("/tmp/gplanner.json");
            //string ser = Newtonsoft.Json.JsonConvert.SerializeObject(translation);
            //System.Console.WriteLine(ser);
            //streamWriter.Write(ser);
            //streamWriter.Flush();

            GardenPlannerSettings.Load();

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
            morado.SetColor(new Cairo.Color(0.9, 0.9, 0.9));
            knoblauch.AddVariety("morado", morado);
            var vallelado = new PlantVariety("Vallelado", "bla");
            vallelado.SetColor(new Cairo.Color(0.9, 0.9, 0.9));
            knoblauch.AddVariety("vallelado", vallelado);

            var nachtschattengew = new PlantFamily("Nachtschattengewächse", "");
            Data.AddFamily("nachtschattengewaechse", nachtschattengew);
            var kartoffeln = new Plant("Kartoffel", "");
            kartoffeln.SetColor(new Cairo.Color(0.2, 0.8, 0.8));
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
            plantingArea.AddVariety(morado, new PlantingInfo() { Count = 3});
            plantingArea.AddVariety(vallelado, new PlantingInfo() { Count = 2 });

            //string s = Newtonsoft.Json.JsonConvert.SerializeObject(Data, Newtonsoft.Json.Formatting.Indented);

            return Data;
        }
    }
}

/*TODO
 * -Bug: If planting has created date X and variety has planting date X+1, todolist shows variety in month X instead of X+1 (but the shown date is X+1)
 * -Fix: Better polygon editing
 * -Fix: Better point/area selecting
 * -Add: MethodArea visualization and editing
 * -Int: maybe change varietykeyseq dictionary to list
 * -Add: translation
 * -Add: Maybe use treeview instead for family plant variety
 * -Add: scrolling
 * -Add: window resize and gui sizes
 * -Add: add (internal) check for compatible families/plants (same as for incomaptible)
 * -Add: add visualization for planting suggestions based on good neighbours + free space + white space
 */
