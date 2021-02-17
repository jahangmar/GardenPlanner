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
using Cairo;

namespace GardenPlanner.Garden
{
    public class Plant : Growable
    {
        public string FamilyID;

        /// <summary>
        /// Varieties of this plant
        /// </summary>
        public Dictionary<string, PlantVariety> Varieties;

        private PlantFamily family;

        public PlantFamily GetFamily()
        {
            if (family == null)
                family = GardenData.LoadedData.GetFamily(FamilyID);
            return family;
        }

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

        public List<PlantVariety> VarietiesAsList() =>
            new List<PlantVariety>(Varieties.Values);

        public string GetImageSurfacePath() => System.IO.Path.Combine(System.IO.Path.Combine(GardenData.GetImagePath(), "plants"), Name.Replace(' ', '_') + ".png");
        private ImageSurface imageSurface = null;

        public bool HasImageSurface() => System.IO.File.Exists(GetImageSurfacePath());

        public ImageSurface GetImageSurface()
        {
            if (imageSurface == null && HasImageSurface())
            {
                    imageSurface = new ImageSurface(GetImageSurfacePath());
            }
            return imageSurface;
        }

        public override bool CheckIncompatibleFamilies(string familyID)
        {
            return this.FamilyID.Equals(familyID) || IncompatibleFamilies.Contains(familyID) || GetFamily().CheckIncompatibleFamilies(familyID);
        }

        public override bool CheckIncompatiblePlants(string plantID)
        {
            return this.ID.Equals(plantID) || IncompatiblePlants.Contains(plantID) || GetFamily().IncompatiblePlants.Contains(plantID);
        }
    }
}
