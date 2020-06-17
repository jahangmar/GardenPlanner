using System;
using System.Collections.Generic;
using Cairo;

namespace GardenPlanner.Garden
{
    /// <summary>
    /// Area in a garden with a shape and methods that can be applied to it
    /// </summary>
    public class GardenArea : GardenDataEntry, GardenDrawable
    {
        private const int LINE_WIDTH = 1;

        /// <summary>
        /// Time this area was created.
        /// </summary>
        public DateTime created;
        public DateTime removed;

        /// <summary>
        /// Identifier of the garden this area belongs to
        /// </summary>
        public string GardenID;

        public GardenShape Shape;

        /// <summary>
        /// Methods applied to a whole <see cref="Garden"/> (e.g. compost application or digging)
        /// or planting methods for <see cref="Planting"/> (e.g. directly sown, fertilized)
        /// or method applied to a specific <see cref="GardenArea"/>
        /// </summary>
        public List<BedMethod> Methods;

        public GardenArea(string name, string description) : base(name, description)
        {
            Methods = new List<BedMethod>();
            Shape = new GardenShape();
        }

        public bool ContainsPointOnEdge(GardenPoint p, int xoffset = 0, int yoffset = 0, double zoom = GardenPoint.STD_ZOOM) =>
            Shape.ContainsPointOnEdge(p, xoffset, yoffset, zoom);

        public virtual void Draw(Context context, int xoffset = 0, int yoffset = 0, double zoom = 1)
        {
            this.Shape.Draw(context, xoffset, yoffset, new Color(0.3, 0.2, 0.2), new Color(0.4, 0.3, 0.3, 0.5), LINE_WIDTH, zoom);
        }
    }
}
