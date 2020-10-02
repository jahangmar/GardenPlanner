using System;
namespace GardenPlanner.Garden
{
    public class BedMethod
    {
        /// <summary>
        /// When was this method applied
        /// </summary>
        public DateTime When;

        /// <summary>
        /// Description of the method (e.g. compost applied, dug with spade, fertilized, directly sown)
        /// </summary>
        public string What;

        public BedMethod(DateTime when, string what)
        {
            When = when;
            What = what;
        }

        public override string ToString() => DateRange.DayMonthDateTimeToString(When) + ": " + What;
    }
}
