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
namespace GardenPlanner
{
    public class EditPlantFamilyWindow : EditAffectableWindow<PlantFamily>
    {
        private readonly PlantFamily Family;
        private readonly bool Create;

        public EditPlantFamilyWindow(PlantFamily family, bool create = false) : base(create ? "Create new family" : "Edit family '" + family.Name + "'", family)
        {
            Family = family;
            Create = create;
        }

        /// <summary>
        /// Shows the window for families that already exist.
        /// </summary>
        public static void ShowWindow(PlantFamily family, bool create = false)
        {
            EditPlantFamilyWindow win = new EditPlantFamilyWindow(family, create);
            win.ShowAll();
        }

        /// <summary>
        /// Shows the window for new families
        /// </summary>
        public static void ShowWindow()
        {
            PlantFamily family = new PlantFamily("", "");
            EditPlantFamilyWindow win = new EditPlantFamilyWindow(family, true);
            win.ShowAll();
        }

        protected PlantFamily ModifyOrCreate(PlantFamily family = null)
        {
            if (family == null)
                family = new PlantFamily("", "");

            return (PlantFamily)base.ModifyOrCreate(family);
        }

        protected override void Delete()
        {
            bool deleted = DeleteDialog(() => GardenData.LoadedData.RemoveFamily(Family), Family.Name);
            if (deleted)
                base.Delete();
        }

        protected override void Save()
        {
            if (NameEntry.Text.Length == 0)
            {
                ShowErrorSave(Family.Name, "The name must not be empty");
                return;
            }

            ModifyOrCreate(Family);

            try
            {
                string id = GardenData.GenID(Family.Name);
                /*
                //if variety already exists it is deleted first before it is added again
                if (!Create)
                {
                    GardenData.LoadedData.RemoveFamily(Family);
                }
                GardenData.LoadedData.AddFamily(id, Family);
                */
                if (Create)
                    GardenData.LoadedData.AddFamily(id, Family);

                ShowSuccessSave(Family.Name);
            }
            catch (GardenDataException)
            {
                ShowErrorSave(Family.Name, "Corrupted Data");
            }
            MainWindow.GetInstance().ReloadFamilies();
        }

        protected override void Info()
        {
            InfoPlantFamilyWindow.ShowWindow(ModifyOrCreate(Family), true, Create);
        }
    }
}
