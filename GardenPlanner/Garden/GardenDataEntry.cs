using System.Collections.Generic;

namespace GardenPlanner.Garden
{
    /// <summary>
    /// An entry of data that can be named and described
    /// </summary>
    public abstract class GardenDataEntry
    {
        /// <summary>
        /// Unique identifier for cross referencing and json entry
        /// </summary>
        public string ID;

        /// <summary>
        /// Real world name of the entry
        /// </summary>
        public string Name;

        /// <summary>
        /// Description of the entry
        /// </summary>
        public string Description;

        public GardenDataEntry(string name, string description)
        {
            Name = name;
            Description = description;
            ID = "";
        }

        protected void AddToDictionary<T> (string key, T Value, Dictionary<string, T> dic)
        {
            if (Value is GardenDataEntry)
                (Value as GardenDataEntry).ID = key;
            dic.Add(key, Value);
        }
    }
}
