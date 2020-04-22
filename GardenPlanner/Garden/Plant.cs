using System.Collections.Generic;
using System;

namespace GardenPlanner.Garden
{
    public class Plant : Growable
    {
        public string FamilyID;

        /// <summary>
        /// Varieties of this plant
        /// </summary>
        public Dictionary<string, PlantVariety> Varieties;

        public FeederType FeederType;

        public Plant(string name, string description) : base(name, description)
        {
            Varieties = new Dictionary<string, PlantVariety>();
        }

        public void AddVariety(string varietyID, PlantVariety variety)
        {
            AddToDictionary(varietyID, variety, Varieties);
            if (FamilyID.Length == 0)
                throw new System.Exception($"{typeof(Plant).Name} must be added to {typeof(PlantFamily).Name} before adding {typeof(PlantVariety).Name}");
            variety.FamilyID = FamilyID;
            if (ID.Length == 0)
                throw new System.Exception($"{typeof(Plant).Name} must be added to {typeof(GardenData).Name} before adding {typeof(PlantVariety).Name}");
            variety.PlantID = ID;
        }

        public bool TryGetVariety(string varietyID, out PlantVariety variety) =>
            Varieties.TryGetValue(varietyID, out variety);

        public void RemoveVariety(string varietyID)
        {
            Varieties.Remove(varietyID);
        }
    }
}
