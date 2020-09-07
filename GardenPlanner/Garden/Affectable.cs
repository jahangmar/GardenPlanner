using System;
using System.Collections.Generic;

namespace GardenPlanner.Garden
{
    public class Affectable : GardenDataEntry
    {
        /// <summary>
        /// Scientific name for the family or plant
        /// </summary>
        public string ScientificName = "";

        /// <summary>
        /// List of good neighbour plants
        /// </summary>
        public List<string> CompatiblePlants = new List<string>();

        /// <summary>
        /// List of good neighbour plant families
        /// </summary>
        public List<string> CompatibleFamilies = new List<string>();

        /// <summary>
        /// List of bad neighbour plants
        /// </summary>
        public List<string> IncompatiblePlants = new List<string>();

        /// <summary>
        /// List of bad neighbour plant families
        /// </summary>
        public List<string> IncompatibleFamilies = new List<string>();

        /// <summary>
        /// List of pests that affect this family or plant
        /// </summary>
        public List<string> Pests = new List<string>();

        /// <summary>
        /// List of diseases that affect this family or plant
        /// </summary>
        public List<string> Diseases = new List<string>();

        public Affectable(string name, string description) : base(name, description)
        {

        }
    }
}
