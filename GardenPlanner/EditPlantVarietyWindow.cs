using Gtk;

using GardenPlanner.Garden;

namespace GardenPlanner
{
    public class EditPlantVarietyWindow : EditGrowableWindow<PlantVariety>
    {
        private readonly PlantVariety Variety;
        private readonly bool Create;

        /*public EditPlantVarietyWindow(Plant plant) : this(new PlantVariety("", ""), true)
        {
            Variety.PlantID = plant.ID;
            Variety.FamilyID = plant.FamilyID;
        }*/

        public EditPlantVarietyWindow(PlantVariety variety, bool create=false) : base(create ? "Create new variety" : "Edit variety '" + variety.Name + "'", variety)
        {
            Variety = variety;
            Create = create;
        }

        /// <summary>
        /// Shows the window for varieties that already exist.
        /// </summary>
        public static void ShowWindow(PlantVariety variety, bool create=false) {
            EditPlantVarietyWindow win = new EditPlantVarietyWindow(variety, create);
            win.ShowAll();
        }

        /// <summary>
        /// Shows the window for new varieties not yet added to a plant
        /// </summary>
        /// <param name="plant">the plant the variety belongs to</param>
        public static void ShowWindow(Plant plant)
        {
            PlantVariety variety = new PlantVariety("", "");
            variety.PlantID = plant.ID;
            variety.FamilyID = plant.FamilyID;
            EditPlantVarietyWindow win = new EditPlantVarietyWindow(variety, true);
            win.ShowAll();
        }

        protected PlantVariety ModifyOrCreate(PlantVariety variety = null)
        {
            if (variety == null)
                variety = new PlantVariety("", "");

            return (PlantVariety) base.ModifyOrCreate(variety);
        }

        protected override void Save()
        {
            if (NameEntry.Text.Length == 0)
            {
                ShowErrorSave(Variety.Name, "The name must not be empty");
                return;
            }

            ModifyOrCreate(Variety);

            try
            {
                Plant plant = GardenData.LoadedData.GetPlant(Variety.FamilyID, Variety.PlantID);

                string id = GardenData.GenID(Variety.Name);

                //if variety already exists it is deleted first before it is added again
                if (!Create)
                {
                    plant.RemoveVariety(Variety.ID);
                }
                plant.AddVariety(id, Variety);

                ShowSuccessSave(Variety.Name);
            }
            catch (GardenDataException)
            {
                ShowErrorSave(Variety.Name, "Corrupted Data");
            }

            MainWindow.GetInstance().ReloadFamilies();
        }

        protected override void Delete()
        {
            bool deleted = DeleteDialog(() => GardenData.LoadedData.RemovePlantVariety(Variety), Variety.Name);
            if (deleted)
                base.Delete();
        }

        protected override void Info()
        {
            InfoPlantVarietyWindow.ShowWindow(ModifyOrCreate(Variety), true, Create);
        }
    }
}
