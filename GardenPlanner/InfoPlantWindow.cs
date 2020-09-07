using System;
using GardenPlanner.Garden;

namespace GardenPlanner
{
    public class InfoPlantWindow : InfoGrowableWindow
    {
        private readonly Plant Plant;
        private readonly bool Create;

        private InfoPlantWindow(Plant plant, bool create, bool isEdited) : base("Info about " + plant.Name, isEdited)
        {
            Plant = plant;
            Create = create;

            AddEntry(plant.Name, infoView.headline);

            PlantFamily family = GardenData.LoadedData.GetFamily(plant.FamilyID);
            AddEntry("Gehört zu "+ family.Name , false);
            AddEntry("(" + plant.ScientificName + ")", infoView.weak);

            AddEntry("Beschreibung: ", false);

            AddEntry(plant.Description, infoView.italic);

            AddEntry("Aussaat draußen: " + plant.PlantOutsideDateRange);

            ApplyTags();
        }

        public static void ShowWindow(Plant plant, bool isEdited = false, bool create = false)
        {
            InfoPlantWindow win = new InfoPlantWindow(plant, create, isEdited);
            win.ShowAll();
        }

        protected override void Edit()
        {
            this.Destroy();
            EditPlantWindow.ShowWindow(Plant, Create);
        }

    }
}
