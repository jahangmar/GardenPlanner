using GardenPlanner.Garden;
namespace GardenPlanner
{
    public class EditPlantWindow : EditGrowableWindow<Plant>
    {
        public EditPlantWindow() : base ("title")
        {
        }

        public override void Delete(Plant plant)
        {
            throw new System.NotImplementedException();
        }

        public override void Save()
        {
            throw new System.NotImplementedException();
        }
    }
}
