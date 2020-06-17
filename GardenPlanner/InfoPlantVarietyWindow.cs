using System;
using GardenPlanner.Garden;

namespace GardenPlanner
{
    public class InfoPlantVarietyWindow : InfoGrowableWindow
    {
        private readonly PlantVariety Variety;
        private readonly bool Create;

        private InfoPlantVarietyWindow(PlantVariety variety, bool create, bool isEdited) : base("Info about "+variety.Name, isEdited)
        {
            Variety = variety;
            Create = create;

            Translation trans = Translation.GetTranslation();

            AddEntry(variety.Name, headline);

            if (Variety.FamilyID == null)
                throw new Exception("familyID is null for some reason");

            Plant plant = GardenData.LoadedData.GetPlant(variety.FamilyID, variety.PlantID);
            AddEntry(plant.Name + trans.VarietyAdd +" ", false); 
            AddEntry("(" + plant.ScientificName + ")", weak);

            AddEntry(trans.Description +": ", false);

            AddEntry(variety.Description, italic);

            AddEntry(trans.SowPlantOutside+": " + variety.PlantOutsideDateRange);

            ApplyTags();
        }

        public static void ShowWindow(PlantVariety variety, bool isEdited = false, bool create=false)
        {
            InfoPlantVarietyWindow win = new InfoPlantVarietyWindow(variety, create, isEdited);
            win.ShowAll();
        }

        protected override void Edit()
        {
            this.Destroy();
            EditPlantVarietyWindow.ShowWindow(Variety, Create);
        }

    }
}
