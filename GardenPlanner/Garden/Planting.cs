﻿using System.Collections.Generic;
using Cairo;

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
        public Dictionary<VarietyKeySeq, int> Varieties;

        private Color Color;
        private readonly Color LINE_COLOR = new Color(0.2, 0.2, 0.2);
        private const int LINE_WIDTH = 1;
        private const float IMAGE_SIZE = 48;

        public Planting(string name, string description) : base(name, description)
        {
            //Varieties = new List<VarietyKeySeq>();
            Varieties = new Dictionary<VarietyKeySeq, int>();
            Color = new Color(0, 0, 0);
        }

        public void AddVarietyKeys(string family, string plant, string variety, int count=0)
        {
            //Varieties.Add(new VarietyKeySeq(family, plant, variety));
            Varieties.Add(new VarietyKeySeq(family, plant, variety), count);
        }

        public void AddVariety(PlantVariety variety, int count = 0) =>
            AddVarietyKeys(variety.FamilyID, variety.PlantID, variety.ID, count);

        public void RemoveVarietyKey(string family, string plant, string variety)
        {
            //Varieties.RemoveAll((VarietyKeySeq obj) => obj.FamilyKey.Equals(family) && obj.PlantKey.Equals(plant) && obj.VarietyKey.Equals(variety));
            Varieties.Remove(new VarietyKeySeq(family, plant, variety));
        }

        public void CalcColor()
        {

            double r = 0, g = 0, b = 0;
            int sum = 0;
            foreach (KeyValuePair<VarietyKeySeq, int> pair in Varieties)
            {
                VarietyKeySeq seq = pair.Key;
                int count = pair.Value;
                PlantVariety variety = GardenData.LoadedData.GetVariety(seq);
                Color color = variety.Color;
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

        public override void Draw(Context context, int xoffset=0, int yoffset=0, double zoom=1)
        {

            Shape.Draw(context, xoffset, yoffset, LINE_COLOR, Color, LINE_WIDTH, zoom);
            if (Varieties.Count > 0)
            {
                int i = 0;
                foreach (KeyValuePair<VarietyKeySeq, int> keyValuePair in Varieties)
                {
                    VarietyKeySeq varietyKeySeq = keyValuePair.Key;
                    int count = keyValuePair.Value;
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
                        context.MoveTo(x, y);
                        context.LineTo(x + imageZoom, y);
                        context.LineTo(x + imageZoom, y + imageZoom);
                        context.LineTo(x, y + imageZoom);
                        context.SetSourceColor(plant.Color);
                        context.Fill();
                    }
                    context.SetSourceColor(new Color(0, 0, 0));
                    context.MoveTo(plantingShapePoint.X + 1 + IMAGE_SIZE * zoom * i, plantingShapePoint.Y + (IMAGE_SIZE / 2) * zoom);
                    context.SetFontSize(20 * zoom);
                    context.ShowText(count + "x");
                    i++;
                }
            }
            /*
            ImageSurface surface = new ImageSurface("/home/jan/Desktop/modauthor.png");//"/home/jan/MonoProjects/GardenPlannner/imgs/onion.jpg");
            Context ctx = Gdk.CairoHelper.Create(GardenDrawingArea.ActiveInstance.GdkWindow);
            ctx.SetSourceSurface(surface, 0, 0);
            ctx.Paint();
            ctx.Dispose();
            */           
        }
    }
}
