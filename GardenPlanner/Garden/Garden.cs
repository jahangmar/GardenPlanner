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
        public Dictionary<string, Planting> Plantings;
        public Dictionary<string, GardenArea> MethodAreas;

        private const double LINE_WIDTH = 1.5;

        public Garden(string name, string description) : base(name, description)
        {
            Plantings = new Dictionary<string, Planting>();
        }

        public virtual void AddPlanting(string key, Planting planting)
        {
            AddToDictionary(key, planting, Plantings);
            if (this.ID.Length == 0)
                throw new System.Exception($"{typeof(Garden).Name} must be added to {typeof(GardenData).Name} before adding {typeof(Planting).Name}");
            planting.GardenID = ID;
        }

        public virtual void RemovePlanting(string key)
        {
            Plantings.Remove(key);
        }

        public void Draw(Context context, int xoffset = 0, int yoffset = 0, double zoom=1)
        {
            this.Shape.Draw(context, xoffset, yoffset, new Color(0, 0, 0), new Color(1, 1, 1), LINE_WIDTH, zoom);
        }
    }
}
