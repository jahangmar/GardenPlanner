using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;

namespace GardenPlanner
{
    public class Translation
    {
        protected static Translation instance;

        protected Translation()
        {

        }

        public static void Load(string tr)
        {
            TextReader treader = new StreamReader(Path.Combine(Path.Combine(MainClass.MAIN_PATH, "translations"), tr));
            JsonReader reader = new JsonTextReader(treader);
            instance = JsonConvert.DeserializeObject<Translation>(treader.ReadToEnd());
        }

        public static Translation GetTranslation()
        {
            if (instance == null)
                instance = BuiltIn["english"];

            return instance;      
        }

        public static Translation GetTranslation(string tr)
        {
            instance = BuiltIn[tr];
            return instance;
        }

        public static Dictionary<string, Translation> BuiltIn = new Dictionary<string, Translation>()
        {
            {"english", new Translation()},
            {"german", new GermanTranslation()}
        };

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
            if (string.IsNullOrEmpty(value))
                return value;
            if (value.Length == 1)
                return value[0].ToString().ToUpper();

            return value[0].ToString().ToUpper() + value.Substring(1);
        }
            
    }
}
