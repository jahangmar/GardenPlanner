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

        public bool unsaved = false;

        public GardenData(string name)
        {
            Gardens = new Dictionary<string, Garden>();
            Families = new Dictionary<string, PlantFamily>();
            this.Name = name;
        }

        public static string GetImagePath() => MainClass.MAIN_PATH + "/" + LoadedData.Name + "/imgs/";

        public static int GetFirstYear() => 2000; //TODO
        public static int GetLastYear() => 2030; //TODO

        public void AddFamily(string key, PlantFamily family)
        {
            Families.Add(key, family);
            family.ID = key;
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

        public static string ErrorMessage;

        public static bool Load(string filename)
        {
            object obj;
            try
            {
                obj = JsonConvert.DeserializeObject(File.ReadAllText(filename));
            }
            catch (JsonException e)
            {
                ErrorMessage = e.Message;
                return false;
            }


            if (obj is GardenData)
            {
                LoadedData = (GardenData)obj;
                LoadedData.unsaved = false;
                return true;
            }
            else
            {
                ErrorMessage = "invalid json object";
            }

            return false;
        }

    }
}
