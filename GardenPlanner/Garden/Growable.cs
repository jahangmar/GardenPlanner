using System;
using System.Collections.Generic;

namespace GardenPlanner.Garden
{
    public abstract class Growable : Affectable
    {

        /// <summary>
        /// Date for when this plant can be sown inside
        /// </summary>
        public DateRange SowInsideDateRange = new DateRange();

        /// <summary>
        /// Date for when this plant can be sown or planted outside
        /// </summary>
        public DateRange PlantOutsideDateRange = new DateRange();

        /// <summary>
        /// Date for when this plant can be harvested
        /// </summary>
        public DateRange HarvestDateRange;

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
