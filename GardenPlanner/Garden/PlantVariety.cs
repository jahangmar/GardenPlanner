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

        public new bool MustBeSownInside { get => GetPlant().MustBeSownInside; }
        public new bool MustBeSownOutside { get => GetPlant().MustBeSownOutside; }
        public new DateRange SowInsideDateRange { get => GetPlant().SowInsideDateRange; }
        public new DateRange PlantOutsideDateRange { get => GetPlant().PlantOutsideDateRange; }
        public new DateRange HarvestDateRange { get => GetPlant().HarvestDateRange; }
        public new bool Annual { get => GetPlant().Annual; }
        public new int DaysUntilGermination { get => GetPlant().DaysUntilGermination; }
        public new int DaysUntilHarvest { get => GetPlant().DaysUntilHarvest; }
        public new int DaysUntilPlantOutside { get => GetPlant().DaysUntilPlantOutside; }
        public new FeederType FeederType { get => GetPlant().FeederType; }
        public new int MinTemp { get => GetPlant().MinTemp; }
        public new int MaxTemp { get => GetPlant().MaxTemp; }

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
