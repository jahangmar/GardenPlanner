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

using System.Collections.Generic;
using System.Linq;
using Cairo;
using Newtonsoft.Json;

namespace GardenPlanner.Garden
{
    /// <summary>
    /// Planting of a specific variety. A planting has an area and methods that were applied in this area.
    /// </summary>
    public class Planting : GardenArea, GardenDrawable
    {
        /// <summary>
        /// List of varieties planted
        /// </summary>
        //public List<VarietyKeySeq> Varieties;
        [JsonConverter(typeof(DictionaryVarietyKeySeqConverter))]
        public Dictionary<VarietyKeySeq, PlantingInfo> Varieties;



        private Color Color;
        private readonly Color LINE_COLOR = new Color(0.2, 0.2, 0.2);
        private const int LINE_WIDTH = 1;
        private const float IMAGE_SIZE = 48;

        [JsonConstructor]
        public Planting(string name, string description) : base(name, description)
        {
            //Varieties = new List<VarietyKeySeq>();
            Varieties = new Dictionary<VarietyKeySeq, PlantingInfo>();
            Color = new Color(0, 0, 0);
        }

        public Planting(string name, string description, int cyear, int cmonth, int ryear, int rmonth) : base(name, description, cyear, cmonth, ryear, rmonth)
        {
            Varieties = new Dictionary<VarietyKeySeq, PlantingInfo>();
            Color = new Color(0, 0, 0);
        }

        public PlantingInfo GetPlantingInfo(PlantVariety variety) =>
            GetPlantingInfo(variety.FamilyID, variety.PlantID, variety.ID);

        public PlantingInfo GetPlantingInfo(string family, string plant, string variety) =>
            GetPlantingInfo(new VarietyKeySeq(family, plant, variety));

        public PlantingInfo GetPlantingInfo(VarietyKeySeq key)
        {
            if (Varieties.ContainsKey(key))
                return Varieties[key];
            else
                return null;
        }

        public void AddVarietyKeys(string family, string plant, string variety, PlantingInfo plantingInfo)
        {
            VarietyKeySeq varietyKeySeq = new VarietyKeySeq(family, plant, variety);
            Varieties.Add(new VarietyKeySeq(family, plant, variety), plantingInfo);
        }

        public void AddVariety(PlantVariety variety, PlantingInfo plantingInfo) =>
            AddVarietyKeys(variety.FamilyID, variety.PlantID, variety.ID, plantingInfo);

        public void RemoveVarietyKey(VarietyKeySeq varietyKeySeq) =>
            Varieties.Remove(varietyKeySeq);

        public void RemoveVarietyKey(string family, string plant, string variety)
        {
            foreach (VarietyKeySeq varietyKeySeq in Varieties.Keys)
                if (varietyKeySeq.FamilyKey.Equals(family) && varietyKeySeq.PlantKey.Equals(plant) && varietyKeySeq.VarietyKey.Equals(variety))
                    Varieties.Remove(varietyKeySeq);
        }

        private void RemoveById(System.Func<VarietyKeySeq,bool> func)
        {
            var list = new List<VarietyKeySeq>(Varieties.Keys);
            foreach (VarietyKeySeq varietyKeySeq in list)
            {
                if (func(varietyKeySeq)) {
                    RemoveVarietyKey(varietyKeySeq);
                }

            }
        }

        /// <summary>
        /// Removes all references to the family
        /// </summary>
        public void RemoveFamily(PlantFamily family) =>
            RemoveById((arg) => arg.FamilyKey.Equals(family.ID));

        /// <summary>
        /// Removes all references to the plant
        /// </summary>
        public void RemovePlant(Plant plant) =>
            RemoveById((arg) => arg.PlantKey.Equals(plant.ID));

        /// <summary>
        /// Removes all references to the variety
        /// </summary>
        public void RemovePlantVariety(PlantVariety variety) =>
            RemoveById((arg) => arg.VarietyKey.Equals(variety.ID));

        public void CalcColor()
        {

            double r = 0, g = 0, b = 0;
            int sum = 0;
            foreach (KeyValuePair<VarietyKeySeq, PlantingInfo> pair in Varieties)
            {
                VarietyKeySeq seq = pair.Key;
                int count = pair.Value.Count;
                PlantVariety variety = GardenData.LoadedData.GetVariety(seq);
                Color color = variety.GetColor();
                r += count * color.R;
                g += count * color.G;
                b += count * color.B;
                sum += count;
            }
            r /= sum;
            g /= sum;
            b /= sum;
            Color = new Color(r, g, b);
        }

        public override bool Consistency()
        {
            bool consistency = true;
            foreach (KeyValuePair<VarietyKeySeq, PlantingInfo> pair in Varieties)
            {
                DateRange range = new DateRange(created.Year, created.Month, removed.Year, removed.Month);
                if (!range.IsDateInRange(pair.Value.PlannedPlantingDate))
                {
                    System.Console.WriteLine("consistency check failed: planned planting date was inconsistent with planting creation/removal date (was: "+pair.Value.PlannedPlantingDate+", range: "+range.ToFullString()+")");
                    pair.Value.PlannedPlantingDate = new System.DateTime(created.Year, created.Month, 1);
                    consistency = false;
                }
                if (!range.IsDateInRange(pair.Value.PlantingDate))
                {
                    System.Console.WriteLine("consistency check failed: actual planting date was inconsistent with planting creation/removal date (was: " + pair.Value.PlantingDate + ", range: " + range.ToFullString() + ")");
                    pair.Value.PlantingDate = new System.DateTime(created.Year, created.Month, 1);
                }
            }
            return consistency;
        }

        public override void Draw(Context context, int xoffset=0, int yoffset=0, double zoom=1, int year=0, int month=0)
        {
            if (!CheckDate(year, month))
                return;

            Shape.Draw(context, xoffset, yoffset, LINE_COLOR, Color, LINE_WIDTH, zoom);
            if (GardenPlannerSettings.GetSettings().ShowAreaImages && Varieties.Count > 0)
            {
                int i = 0;
                foreach (KeyValuePair<VarietyKeySeq, PlantingInfo> keyValuePair in Varieties)
                {
                    VarietyKeySeq varietyKeySeq = keyValuePair.Key;
                    int count = keyValuePair.Value.Count;
                    Plant plant = GardenData.LoadedData.GetPlant(varietyKeySeq.FamilyKey, varietyKeySeq.PlantKey);
                    PlantVariety variety = GardenData.LoadedData.GetVariety(varietyKeySeq);

                    PointD plantingShapePoint = Shape.GetTopLeftPoint().ToCairoPointD(xoffset, yoffset, zoom);

                    //Draw image or substitute
                    if (plant.HasImageSurface())
                    {
                        ImageSurface surf = plant.GetImageSurface();
                        int surfw = surf.Width;
                        int surfh = surf.Height;
                        context.Save();

                        double scaleH = (IMAGE_SIZE / surfw) * zoom;
                        double scaleV = (IMAGE_SIZE / surfh) * zoom;
                        context.Translate(plantingShapePoint.X+1 + scaleH * surfw * i, plantingShapePoint.Y+1);

                        context.Scale(scaleH, scaleV);

                        context.SetSourceSurface(surf, 0, 0);

                        context.Paint();
                        context.Restore();

                    }
                    else
                    {
                        double imageZoom = IMAGE_SIZE * zoom;
                        double x = plantingShapePoint.X + 1 + imageZoom * i;
                        double y = plantingShapePoint.Y + 1;
                        context.SetSourceColor(plant.GetColor());
                        context.MoveTo(x, y);
                        context.LineTo(x + imageZoom, y);
                        context.LineTo(x + imageZoom, y + imageZoom);
                        context.LineTo(x, y + imageZoom);
                        context.LineTo(x, y);

                        context.Fill();
                    }

                    //Draw amount
                    context.SetSourceColor(new Color(0, 0, 0));
                    context.MoveTo(plantingShapePoint.X + 1 + IMAGE_SIZE * zoom * i, plantingShapePoint.Y + (IMAGE_SIZE / 2) * zoom);
                    context.SetFontSize(20 * zoom);
                    context.ShowText(count + "x");

                    //Draw Name
                    if (GardenPlannerSettings.GetSettings().ShowPlantNames || GardenPlannerSettings.GetSettings().ShowVarietyNames)
                    {
                        context.SetSourceColor(new Color(0, 0, 0));
                        context.MoveTo(plantingShapePoint.X + 1 + IMAGE_SIZE * zoom * i, plantingShapePoint.Y + (1.25 * IMAGE_SIZE) * zoom);
                        context.SetFontSize(18 * zoom);
                        context.Rotate(0.45);
                        string text = "";
                        if (GardenPlannerSettings.GetSettings().ShowPlantNames && GardenPlannerSettings.GetSettings().ShowVarietyNames)
                        {
                            text = variety.Name + " (" + plant.Name + ")";
                        }
                        else
                        {
                            text = GardenPlannerSettings.GetSettings().ShowPlantNames ? plant.Name : variety.Name;
                        }
                        context.ShowText(text);
                        context.Rotate(-0.45);
                    }

                    i++;
                }
            }      
        }

        public override List<string> GetTodoList(DateRange range)
        {
            List<string> result = new List<string>();

            string PlantingName = Name;

            if (range.IsDateInRange(created))
            {
                foreach (KeyValuePair<VarietyKeySeq, PlantingInfo> pair in Varieties)
                {
                    VarietyKeySeq varietyKeySeq = pair.Key;
                    PlantingInfo plantingInfo = pair.Value;
                    Plant plant = GardenData.LoadedData.GetPlant(varietyKeySeq);
                    string sown = plant.MustBeSownOutside ? "sow" : "plant";
                    string strike = plantingInfo.AlreadyPlanted ? "#" : ""; //added as prefix to indicate that this entry should be marked as done
                    result.Add($"{strike}{DateRange.ApproxDayMonthDateTimeToString(plantingInfo.PlannedPlantingDate)}: {sown} {GardenData.LoadedData.GetVariety(varietyKeySeq).Name}");
                }
                //if (Varieties.Count > 0)
                //    result.Add(DateRange.ApproxDayMonthDateTimeToString(created)+": plant "+ Varieties.Keys.ToList().ConvertAll((VarietyKeySeq input) => GardenData.LoadedData.GetVariety(input).Name).Aggregate((string elem1, string elem2) => elem1 + ", " + elem2)+ " in "+PlantingName);
            }

            foreach (VarietyKeySeq varietyKeySeq in Varieties.Keys)
            {
                PlantVariety variety = GardenData.LoadedData.GetVariety(varietyKeySeq);
                PlantingInfo plantingInfo = Varieties[varietyKeySeq];
                System.DateTime plantTime = plantingInfo.PlannedPlantingDate.AddDays(-variety.DaysUntilPlantOutside);
                if (variety.MustBeSownInside && range.IsDateInRange(plantTime))
                {
                    string strike = plantingInfo.AlreadyPlanted ? "#" : ""; //added as prefix to indicate that this entry should be marked as done
                    result.Add(strike + DateRange.ApproxDayMonthDateTimeToString(plantTime) + ": sow " + variety.Name + " inside");
                }
            }

            return result;
        }
    }
}
