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

        public void AddVarietyKeys(string family, string plant, string variety, int count=0)
        {
            VarietyKeySeq varietyKeySeq = new VarietyKeySeq(family, plant, variety);
            Varieties.Add(new VarietyKeySeq(family, plant, variety), new PlantingInfo() { Count = count });
        }

        public void AddVariety(PlantVariety variety, int count = 0) =>
            AddVarietyKeys(variety.FamilyID, variety.PlantID, variety.ID, count);

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

                    PointD plantingShapePoint = Shape.GetTopLeftPoint().ToCairoPointD(xoffset, yoffset, zoom);

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

                        //System.Console.WriteLine("drawing" + plant.Name);
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

                        context.Fill();
                    }
                    context.SetSourceColor(new Color(0, 0, 0));
                    context.MoveTo(plantingShapePoint.X + 1 + IMAGE_SIZE * zoom * i, plantingShapePoint.Y + (IMAGE_SIZE / 2) * zoom);
                    context.SetFontSize(20 * zoom);
                    context.ShowText(count + "x");
                    i++;
                }
            }      
        }

        public override List<string> GetTodoList(DateRange range)
        {
            List<string> result = new List<string>();

            if (range.IsDateInRange(created))
            {
                if (Varieties.Count > 0)
                    result.Add(DateRange.ApproxDayMonthDateTimeToString(created)+": plant "+ Varieties.Keys.ToList().ConvertAll((VarietyKeySeq input) => GardenData.LoadedData.GetVariety(input).Name).Aggregate((string arg1, string arg2) => arg1 + ", " + arg2)+ " in "+Name);
            }

            foreach (VarietyKeySeq varietyKeySeq in Varieties.Keys)
            {
                PlantVariety variety = GardenData.LoadedData.GetVariety(varietyKeySeq);
                System.DateTime dateTime = created.AddDays(-variety.DaysUntilPlantOutside);
                if (variety.MustBeSownInside && range.IsDateInRange(created.AddDays(-variety.DaysUntilPlantOutside)))
                {
                    result.Add(DateRange.ApproxDayMonthDateTimeToString(dateTime) + ": sow " + variety.Name + " inside");
                }
            }

            return result;
        }
    }
}
