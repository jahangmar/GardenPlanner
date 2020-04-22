using GardenPlanner.Garden;

namespace GardenPlanner
{
    public abstract class EditGrowableWindow<T> : EditAffectableWindow<T>
    {
        public EditGrowableWindow(string title) : base(title)
        {
        }
    }
}
