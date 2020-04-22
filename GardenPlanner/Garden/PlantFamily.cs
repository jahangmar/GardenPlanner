using System.Collections.Generic;
using System;

namespace GardenPlanner.Garden
{
    public class PlantFamily : Affectable
    {
        /// <summary>
        /// Plants belonging to this family
        /// </summary>
        public Dictionary<string, Plant> Plants;

        public PlantFamily(string name, string description) : base(name, description)
        {
            Plants = new Dictionary<string, Plant>();
        }

        public void AddPlant(string plantID, Plant plant)
        {
            AddToDictionary(plantID, plant, Plants);
            if (this.ID.Length == 0)
                throw new System.Exception(typeof(PlantFamily).Name + " must be added to " + typeof(GardenData).Name + " before adding " + typeof(Plant).Name);
            plant.FamilyID = ID;
        }

        public bool TryGetPlant(string plantID, out Plant plant) =>
            Plants.TryGetValue(plantID, out plant);

        public void RemovePlant(string plantID)
        {
            Plants.Remove(plantID);
        }
    }
}
