using GardenPlanner.Garden;
namespace GardenPlanner
{
    public class EditPlantFamilyWindow : EditAffectableWindow<PlantFamily>
    {
        public EditPlantFamilyWindow() : base("title")
        {
        }

        public override void Delete(PlantFamily affectable)
        {
            throw new System.NotImplementedException();
        }

        public override void Save()
        {
            throw new System.NotImplementedException();
        }
    }
}
