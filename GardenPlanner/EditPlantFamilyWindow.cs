using GardenPlanner.Garden;
namespace GardenPlanner
{
    public class EditPlantFamilyWindow : EditAffectableWindow<PlantFamily>
    {
        public EditPlantFamilyWindow() : base("title")
        {
        }

        protected override void Delete(PlantFamily affectable)
        {
            throw new System.NotImplementedException();
        }

        protected override void Save()
        {
            throw new System.NotImplementedException();
        }
        protected override void Info()
        {
            throw new System.NotImplementedException();
        }
    }
}
