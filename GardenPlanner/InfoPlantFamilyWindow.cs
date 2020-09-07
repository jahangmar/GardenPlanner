using System;
using GardenPlanner.Garden;

namespace GardenPlanner
{
    public class InfoPlantFamilyWindow : InfoAffectableWindow
    {
        private readonly PlantFamily Family;
        private readonly bool Create;

        private InfoPlantFamilyWindow(PlantFamily family, bool create, bool isEdited) : base("Info about " + family.Name, isEdited)
        {
            Family = family;
            Create = create;

            AddEntry(family.Name, infoView.headline);

            AddEntry("(" + family.ScientificName + ")", infoView.weak);

            AddEntry("Beschreibung: ", false);

            AddEntry(family.Description, infoView.italic);

            ApplyTags();
        }

        public static void ShowWindow(PlantFamily family, bool isEdited = false, bool create = false)
        {
            InfoPlantFamilyWindow win = new InfoPlantFamilyWindow(family, create, isEdited);
            win.ShowAll();
        }

        protected override void Edit()
        {
            this.Destroy();
            EditPlantFamilyWindow.ShowWindow(Family, Create);
        }

    }
}
