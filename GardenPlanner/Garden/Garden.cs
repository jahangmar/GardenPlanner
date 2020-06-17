using System.Collections.Generic;
using System;
using Cairo;

namespace GardenPlanner.Garden
{
    /// <summary>
    /// A garden. This can be an area with multiple beds. Beds that are far away should be included in their own garden. A garden has plantings and methods.
    /// </summary>
    public class Garden : GardenArea, GardenDrawable
    {
        public Dictionary<string, Planting> Plantings = new Dictionary<string, Planting>();
        public Dictionary<string, GardenArea> MethodAreas = new Dictionary<string, GardenArea>();

        private const double LINE_WIDTH = 2;

        public Garden(string name, string description) : base(name, description)
        {

        }

        public void AddPlanting(string key, Planting planting)
        {
            AddToDictionary(key, planting, Plantings);
            if (this.ID.Length == 0)
                throw new System.Exception($"{typeof(Garden).Name} must be added to {typeof(GardenData).Name} before adding {typeof(Planting).Name}");
            planting.GardenID = ID;
        }

        public void AddMethodArea(string key, GardenArea methodArea)
        {
            AddToDictionary(key, methodArea, MethodAreas);
            if (this.ID.Length == 0)
                throw new System.Exception($"{typeof(Garden).Name} must be added to {typeof(GardenData).Name} before adding {typeof(GardenArea).Name}");
            methodArea.GardenID = ID;
        }

        public void RemovePlanting(string key)
        {
            RemFromDictionary(key, Plantings);
        }

        public void RemovePlanting(Planting planting)
        {
            RemFromDictionary(planting, Plantings);
        }

        public void RemoveMethodArea(string key)
        {
            RemFromDictionary(key, MethodAreas);
        }

        public void RemoveMethodArea(GardenArea area)
        {
            RemFromDictionary(area, MethodAreas);
        }

        public override void Draw(Context context, int xoffset = 0, int yoffset = 0, double zoom=1)
        {
            this.Shape.Draw(context, xoffset, yoffset, new Color(0, 0, 0), new Color(1, 1, 1), LINE_WIDTH, zoom);
            foreach (KeyValuePair<string, Planting> pair in Plantings)
            {
                pair.Value.Draw(context, xoffset, yoffset, zoom);
            }

            foreach (KeyValuePair<string, GardenArea> pair in MethodAreas)
            {
                pair.Value.Draw(context, xoffset, yoffset, zoom);
            }
        }
    }
}
