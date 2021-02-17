// Copyright (c) 2020 Jahangmar
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <https://www.gnu.org/licenses/>.
//

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

        public List<Plant> GetPlantsAsList() =>
            new List<Plant>(Plants.Values);

        public override bool CheckIncompatibleFamilies(string familyID)
        {
            return familyID.Equals(ID) || IncompatibleFamilies.Contains(familyID);
        }

        public override bool CheckIncompatiblePlants(string plantID)
        {
            return IncompatiblePlants.Contains(plantID);
        }
    }
}
