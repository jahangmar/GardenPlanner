using System;
namespace GardenPlanner
{
    public abstract class InfoGrowableWindow : InfoAffectableWindow
    {
        public InfoGrowableWindow(string title, bool isEdited) : base(title, isEdited)
        {
        }
    }
}
