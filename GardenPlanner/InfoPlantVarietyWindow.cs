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

            AddEntry(variety.Name, infoView.headline);

            Plant plant = GardenData.LoadedData.GetPlant(variety.FamilyID, variety.PlantID);
            AddEntry(plant.Name + "-Sorte" +" ", false); 
            AddEntry("(" + plant.ScientificName + ")", infoView.weak);

            AddEntry("Beschreibung: ", false);

            AddEntry(variety.Description, infoView.italic);

            AddEntry("Aussaat draußen: " + variety.PlantOutsideDateRange);

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
