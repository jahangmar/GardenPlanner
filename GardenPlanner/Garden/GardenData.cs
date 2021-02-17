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
using System.IO;
using Newtonsoft.Json;

namespace GardenPlanner.Garden
{
    public class GardenData
    {
        public static GardenData LoadedData = new GardenData("default");

        public Dictionary<string, Garden> Gardens;
        public Dictionary<string, PlantFamily> Families;

        public string Name;

        [JsonIgnore]
        public static bool unsaved = false;

        public GardenData(string name)
        {
            Gardens = new Dictionary<string, Garden>();
            Families = new Dictionary<string, PlantFamily>();
            this.Name = name;
        }

        public static string GetImagePath() => System.IO.Path.Combine(MainClass.MAIN_PATH,"imgs");

        public static int GetFirstYear() => 2000; //TODO
        public static int GetLastYear() => 2030; //TODO

        public void AddFamily(string key, PlantFamily family)
        {
            Families.Add(key, family);
            family.ID = key;
        }

        public void RemoveFamily(string key)
        {
            Families.Remove(key);
        }

        /// <summary>
        /// Removes a plant family from the family collection and removes all references to plants and varieties in this family
        /// </summary>
        public void RemoveFamily(PlantFamily family)
        {
            RemoveFamily(family.ID);
            foreach (Garden garden in Gardens.Values)
                foreach (Planting planting in garden.Plantings.Values)
                    planting.RemoveFamily(family);
        }

        /// <summary>
        /// Removes all references to a plant
        /// </summary>
        public void RemovePlant(Plant plant)
        {
            GetFamily(plant.FamilyID).RemovePlant(plant.ID);

            foreach (Garden garden in Gardens.Values)
                foreach (Planting planting in garden.Plantings.Values)
                    planting.RemovePlant(plant);
        }

        /// <summary>
        /// Removes all references to a variety
        /// </summary>
        public void RemovePlantVariety(PlantVariety variety)
        {
            GetFamily(variety.FamilyID).Plants[variety.PlantID].RemoveVariety(variety.ID);

            foreach (Garden garden in Gardens.Values)
                foreach (Planting planting in garden.Plantings.Values)
                    planting.RemovePlantVariety(variety);
        }

        public void AddGarden(string key, Garden garden)
        {
            Gardens.Add(key, garden);
            garden.ID = key;
        }

        public PlantFamily GetFamily(string familyID)
        {
            PlantFamily family;
            if (Families.TryGetValue(familyID, out family))
                return family;
            else
                throw new GardenDataException("familyID not found: " + familyID);
        }

        public Plant GetPlant(VarietyKeySeq varietyKeySeq) =>
            GetPlant(varietyKeySeq.FamilyKey, varietyKeySeq.PlantKey);

        public Plant GetPlant(string familyID, string plantID)
        {
            PlantFamily family = GetFamily(familyID);
            Plant plant;
            if (family.TryGetPlant(plantID, out plant))
                return plant;
            else
                throw new GardenDataException("plantID not found: " + plantID);
        }

        /// <exception cref="GardenDataException"></exception>
        public PlantVariety GetVariety(string familyID, string plantID, string varietyID)
        {
            Plant plant = GetPlant(familyID, plantID);
            PlantVariety variety;
            if (plant.TryGetVariety(varietyID, out variety))
                return variety;
            else
                throw new GardenDataException("varietyID not found: " + varietyID);
        }

        public PlantVariety GetVariety(VarietyKeySeq varietyKeySeq) =>
            GetVariety(varietyKeySeq.FamilyKey, varietyKeySeq.PlantKey, varietyKeySeq.VarietyKey);

        public List<PlantFamily> GetFamiliesAsList() => new List<PlantFamily>(Families.Values);

        public static string ErrorMessage;

        public static bool Load(string filename)
        {
            GardenData obj;
            try
            {
                obj = JsonConvert.DeserializeObject<GardenData>(File.ReadAllText(filename));
            }
            catch (System.Exception e)
            {
                ErrorMessage = e.Message;
                return false;
            }

            LoadedData = obj;
            GardenData.unsaved = false;
            if (!LoadedData.Consistency())
            {
                System.Console.WriteLine("Consistency check failed");
            }
            return true;
        }

        public static bool Save(string filename)
        {

            string name = filename.Substring(filename.LastIndexOf('/')+1);
            name = name.LastIndexOf('.') > 0 ? name.Substring(0, name.LastIndexOf('.')) : name;
            LoadedData.Name = name;

            try
            {
                File.WriteAllText(filename, JsonConvert.SerializeObject(LoadedData, Formatting.Indented));
            }
            catch (System.Exception e)
            {
                ErrorMessage = e.Message;
                return false;
            }

            return true;
        }

        public static string GenID(string name)
        {
            System.Random random = new System.Random();

            string s;

            bool IdExists()
            {
                return LoadedData.Families.ContainsKey(s) || LoadedData.Gardens.ContainsKey(s);
                //TODO other dictionaries
            }

            do {
                s = name.Replace(' ', '_') + "_" + random.Next();
            } while (IdExists());

            return s;
        }

        public bool Consistency()
        {
            bool consistency = true;
            foreach (KeyValuePair<string, Garden> pair in Gardens)
            {
                if (!pair.Value.Consistency())
                    consistency = false;
            }
            return consistency;
        }
    }
}
