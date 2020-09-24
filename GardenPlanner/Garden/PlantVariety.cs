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

        private Plant plant;

        private Plant GetPlant()
        {
            if (plant == null)
            {
                plant = GardenData.LoadedData.GetPlant(FamilyID, PlantID);
            }
            return plant;
        }

        public PlantVariety(string name, string description): base(name, description)
        {

        }

        public override bool CheckIncompatibleFamilies(string familyID)
        {
            return GetPlant().CheckIncompatibleFamilies(familyID);
        }

        public override bool CheckIncompatiblePlants(string plantID)
        {
            return GetPlant().CheckIncompatiblePlants(plantID);
        }
    }
}
