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

using GardenPlanner.Garden;
using Gtk;

namespace GardenPlanner
{
    public class EditPlantWindow : EditGrowableWindow<Plant>
    {
        private readonly Plant Plant;
        private readonly bool Create;

        private ComboBoxEntry FeederComboBox;

        public EditPlantWindow(Plant plant, bool create = false) : base(create ? "Create new plant" : "Edit plant '" + plant.Name + "'", plant)
        {
            Plant = plant;
            Create = create;

            FeederComboBox = new ComboBoxEntry(System.Enum.GetNames(typeof(FeederType)));
            FeederComboBox.Active = (int)plant.FeederType;
            AddEntry("Feeder", FeederComboBox);
        }

        /// <summary>
        /// Shows the window for plants that already exist.
        /// </summary>
        public static void ShowWindow(Plant plant, bool create = false)
        {
            EditPlantWindow win = new EditPlantWindow(plant, create);
            win.ShowAll();
        }

        /// <summary>
        /// Shows the window for new plants not yet added to a family
        /// </summary>
        /// <param name="family">the family the plant belongs to</param>
        public static void ShowWindow(PlantFamily family)
        {
            Plant plant = new Plant("", "");
            plant.FamilyID = family.ID;
            EditPlantWindow win = new EditPlantWindow(plant, true);
            win.ShowAll();
        }

        protected Plant ModifyOrCreate(Plant plant = null)
        {
            if (plant == null)
                plant = new Plant("", "");

            plant.FeederType = (FeederType) FeederComboBox.Active;

            return (Plant)base.ModifyOrCreate(plant);
        }

        protected override void Delete()
        {
            bool deleted = DeleteDialog(() => GardenData.LoadedData.RemovePlant(Plant), Plant.Name);
            if (deleted)
                base.Delete();
        }

        protected override void Save()
        {
            if (NameEntry.Text.Length == 0)
            {
                ShowErrorSave(Plant.Name, "The name must not be empty");
                return;
            }

            ModifyOrCreate(Plant);

            try
            {
                PlantFamily family = GardenData.LoadedData.GetFamily(Plant.FamilyID);

                string id = GardenData.GenID(Plant.Name);

                /*
                //if variety already exists it is deleted first before it is added again
                if (!Create)
                {
                    family.RemovePlant(Plant.ID);
                }
                family.AddPlant(id, Plant);
                */

                if (Create)
                    family.AddPlant(id, Plant);

                ShowSuccessSave(Plant.Name);
            }
            catch (GardenDataException)
            {
                ShowErrorSave(Plant.Name, "Corrupted Data");
            }

            MainWindow.GetInstance().ReloadFamilies();
        }

        protected override void Info()
        {
            InfoPlantWindow.ShowWindow(ModifyOrCreate(Plant), true, Create);
        }
    }
}
