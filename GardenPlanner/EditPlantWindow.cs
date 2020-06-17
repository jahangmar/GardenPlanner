using GardenPlanner.Garden;
namespace GardenPlanner
{
    public class EditPlantWindow : EditGrowableWindow<Plant>
    {
        public EditPlantWindow() : base ("title")
        {
        }

        protected override void Delete(Plant plant)
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
