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

        public bool MustBeSownInside = false;

        public bool MustBeSownOutside = false;

        /// <summary>
        /// Date for when this plant can be harvested
        /// </summary>
        public DateRange HarvestDateRange;

        public bool Annual = true;

        /// <summary>
        /// Number of days it takes from sowing to germination.
        /// </summary>
        public int DaysUntilGermination = 0;

        /// <summary>
        /// Number of days it takes from sowing to planting outside.
        /// </summary>
        public int DaysUntilPlantOutside = 0;

        /// <summary>
        /// Number of days it takes from planting or sowing outside to harvest
        /// </summary>
        public int DaysUntilHarvest = 0;

        /// <summary>
        /// Indicates if the plant is a heavy, medium or light feeder.
        /// </summary>
        public FeederType FeederType = FeederType.Heavy;

        /// <summary>
        /// Minimal temperature this plant can survive with or grow without problems.
        /// </summary>
        public int MinTemp = 0;

        /// <summary>
        /// Maximal temperature this plant can grow without problems with.
        /// </summary>
        public int MaxTemp = 30;

        public Cairo.Color Color = new Cairo.Color(1, 1, 1);

        public Growable(string name, string description) : base(name, description)
        {

        }

    }
}
