using Newtonsoft.Json;
using System.IO;

namespace GardenPlanner
{
    public class Translation
    {
        public static Translation Load()
        {
            TextReader treader = new StreamReader("/tmp/gplanner.json");
            JsonReader reader = new JsonTextReader(treader);
            return JsonConvert.DeserializeObject<Translation>(treader.ReadToEnd());
        }

        //private static Translation instance;

        public static Translation GetTranslation()
        {
            return new Translation();
            /*
            if (instance == null)
                instance = Load();

            return instance;
            */           
        }

        //General
        public string Plant = "plant";
        public string Family = "plant family";
        public string Variety = "plant variety";
        public string Sow = "sow";
        public string PlantVerb = "plant";
        public string From = "from";
        public string To = "to";
        public string SowOutside = "sow outside";
        public string SowInside = "sow inside";
        public string PlantOutside = "plant outside";
        public string Harvest = "harvest";
        public string Unset = "<unset>";
        public string SowPlantOutside = "sow/plant outside";
        public string VarietyAdd = " variety";

        //GardenDataEntry
        public string Name = "name";
        public string Description = "description";

        //Affectable
        public string SciName = "scientific name";

        //Growable

        //Dates
        public string[] Dates_Months = { "<Dates_Month[0]>", "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
        public string Dates_Beginning = "beginning of";
        public string Dates_Middle = "middle of";
        public string Dates_End = "end of";

    }

    public static class TranslationStringExtension
    {
        public static string Upper0(this string value) {
            if (value == null || value.Length == 0)
                return value;
            if (value.Length == 1)
                return value[0].ToString().ToUpper();

            return value[0].ToString().ToUpper() + value.Substring(1);
        }
            
    }
}
