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
using System;
using System.IO;
using Newtonsoft.Json;

namespace GardenPlanner
{
    public class GardenPlannerSettings
    {
        private static GardenPlannerSettings instance = new GardenPlannerSettings();

        public static GardenPlannerSettings GetSettings() => instance;

        private static readonly string path = Path.Combine(MainClass.MAIN_PATH, "settings.json");

        public static void Load()
        {
            if (File.Exists(path))
            {
                TextReader treader = new StreamReader(path);
                JsonReader reader = new JsonTextReader(treader);
                instance = JsonConvert.DeserializeObject<GardenPlannerSettings>(treader.ReadToEnd());
                if (instance == null)
                    instance = new GardenPlannerSettings();
                reader.Close();
            }
        }

        public static void Save()
        {
            StreamWriter streamWriter = new StreamWriter(path);
            streamWriter.Write(JsonConvert.SerializeObject(instance));
            streamWriter.Close();
        }

        public string Language = "english";
        public bool ShowAreaImages = true;
        public bool ShowPlantNames = false;
        public bool ShowVarietyNames = false;
        public int MinYear = 2000;
        public int MaxYear = 2100;
    }
}
