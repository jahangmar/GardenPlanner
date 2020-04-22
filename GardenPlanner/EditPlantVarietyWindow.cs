using Gtk;

using GardenPlanner.Garden;

namespace GardenPlanner
{
    public class EditPlantVarietyWindow : EditGrowableWindow<PlantVariety>
    {
        Entry NameEntry;
        TextView DescriptionTextView;

        private readonly PlantVariety Variety;
        private readonly bool Create;

        public EditPlantVarietyWindow(Plant plant) : this(new PlantVariety("", ""), true)
        {
            Variety.PlantID = plant.ID;
            Variety.FamilyID = plant.FamilyID;
        }

        public EditPlantVarietyWindow(PlantVariety variety, bool create=false) : base(create ? "Create new variety" : "Edit variety '" + variety.Name + "'")
        {
            Variety = variety;
            Create = create;

            NameEntry = new Entry(variety.Name);
            AddEntry("Name", NameEntry);
            //SciNameEntry = new Entry(variety.ScientificName);
            //AddEntry("Scientific Name", SciNameEntry);
            DescriptionTextView = new TextView();
            DescriptionTextView.Buffer.Text = variety.Description;
            AddEntry("Description ", DescriptionTextView);


        }

        public override void Save()
        {
            //TODO do the actual saving

            if (NameEntry.Text.Length == 0)
            {
                ShowErrorSave(Variety.Name, "The name has not to be empty");
                return;
            }

            Variety.Name = NameEntry.Text;
            Variety.Description = DescriptionTextView.Buffer.Text;

            try
            {
                Plant plant = GardenData.LoadedData.GetPlant(Variety.FamilyID, Variety.PlantID);

                string id = "variety_" + Variety.Name.Replace(' ', '-');

                //if variety already exists it is deleted first before it is added again
                if (!Create)
                {
                    plant.RemoveVariety(Variety.ID);
                }
                //if it was created but has the same name (id) as another variety
                else if (plant.TryGetVariety(id, out PlantVariety variety))
                {
                    ShowErrorSave(Variety.Name, "A variety with the same name already exists");
                    return;
                }
                    plant.AddVariety("variety_" + Variety.Name.Replace(' ', '-'), Variety);

                    ShowSuccessSave(Variety.Name);
            }
            catch (GardenDataException)
            {
                ShowErrorSave(Variety.Name, "Corrupted Data");
            }
        }

        public override void Delete(PlantVariety variety)
        {
            throw new System.NotImplementedException();
        }
    }
}
