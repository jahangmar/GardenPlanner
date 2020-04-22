using System;
using System.Collections.Generic;

namespace GardenPlanner.Garden
{
    /// <summary>
    /// Area in a garden with a shape and methods that can be applied to it
    /// </summary>
    public class GardenArea : GardenDataEntry
    {
        /// <summary>
        /// Time this area was created. For Areas in a garden this can mean that an area
        /// </summary>
        public DateTime created;
        public DateTime removed;

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
}
}
