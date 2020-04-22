using System;
using System.Collections.Generic;

namespace GardenPlanner.Garden
{
    public abstract class Growable : Affectable
    {
        /// <summary>
        /// Start date for when this plant can be sown or planted
        /// </summary>
        public DateTime PlantDateStart;

        /// <summary>
        /// End date for when this plant can be sown or planted
        /// </summary>
        public DateTime PlantDateEnd;

        /// <summary>
        /// Start date for when this plant can be harvested
        /// </summary>
        public DateTime HarvestDateStart;

        /// <summary>
        /// End date for when this plant can be harvested
        /// </summary>
        public DateTime HarvestDateEnd;

        /// <summary>
        /// Number of days it takes from planting or sowing to harvest
        /// </summary>
        public int DaysUntilHarvest;

        public Cairo.Color Color = new Cairo.Color(0, 0, 0);

        public Growable(string name, string description) : base(name, description)
        {

        }
    }
}
