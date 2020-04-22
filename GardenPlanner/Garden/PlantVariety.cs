using System;
namespace GardenPlanner.Garden
{
    /// <summary>
    /// Variety of a plant
    /// </summary>
    public class PlantVariety : Growable
    {
        public string FamilyID;
        public string PlantID;

        public PlantVariety(string name, string description): base(name, description)
        {

        }
    }
}
