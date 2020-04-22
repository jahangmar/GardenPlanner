using System.Collections.Generic;
using GardenPlanner;
using GardenPlanner.Garden;

using Gtk;

public partial class MainWindow : Window
{
    MenuBar MenuBar = new MainWindowMenuBar();
    Notebook GardenBedBook = new Notebook();
    HPaned HPane = new HPaned();
    VPaned VPane = new VPaned();
    VPaned PlantVarietySelector = new VPaned();
    VPaned FamilyPlantVarietySelector = new VPaned();
    ComboBox VarietyBox;
    HButtonBox ToolBox = new HButtonBox();
    Button EditButton = new Button("Edit");

    SpinButton ZoomButton = new SpinButton(0.1, 10, 0.1);
    //Dictionary<string, Dictionary<string, ComboBox>> VarietyBoxes;
    //Dictionary<string, ComboBox> PlantBoxes;

    //List<List<ComboBox>> VarietyBoxes = new List<List<ComboBox>>();

    //List<ComboBox> PlantBoxes = new List<ComboBox>();
    //VBox FamilyBox = new VBox();
    //ComboBox FamilyBox;
    private List<string> familyIDs = new List<string>();
    private List<string> plantIDs = new List<string>();
    private List<string> varietyIDs = new List<string>();

    //GardenDataEntry SelectedEntry = null;

    private string[] empty = { };

    private static readonly MainWindow instance = new MainWindow();
    public static MainWindow GetInstance() => instance;

    public MainWindow() : base(WindowType.Toplevel)
    {
        GardenData.LoadedData = GardenPlanner.MainClass.TestData();

        HPane.Add1(GardenBedBook);
        HPane.Add2(FamilyPlantVarietySelector);


        VPane.Add1(HPane);
        VPane.Add2(ToolBox);

        ToolBox.Add(EditButton);

        //TODO add more actions for other widgets or save selected entry so other widgets know what is currently selected
        EditButton.Clicked += (object sender, System.EventArgs e) =>
        {
            System.Console.WriteLine("TODO: Edit " + SelectedEntry?.Name); //TODO
            if (SelectedEntry is PlantFamily family)
            { }
            else if (SelectedEntry is Plant plant)
            { }
            else if (SelectedEntry is PlantVariety variety)
            {
                EditPlantVarietyWindow win = new EditPlantVarietyWindow(variety);
                win.ShowAll();
            }
        };



        ZoomButton.Value = GardenDrawingArea.Zoom;
        ZoomButton.Events = Gdk.EventMask.ButtonPressMask | Gdk.EventMask.ButtonReleaseMask | Gdk.EventMask.KeyPressMask | Gdk.EventMask.KeyReleaseMask;
        ZoomButton.TooltipText = "Zoom";

        ToolBox.Add(ZoomButton);

        //FamilyPlantVarietySelector.Add1(PopulateFamilies(GardenData.LoadedData)); 
        RepopulateGrowables();
       
        foreach (KeyValuePair<string, Garden> pair in GardenData.LoadedData.Gardens)
        {
            string key = pair.Key;
            Garden garden = pair.Value;

            GardenBedBook.AppendPage(new GardenDrawingArea(garden, ZoomButton), new Label(garden.Name));
            System.Console.WriteLine("Added page for " + garden.Name);
        }
        //this.Add(MenuBar);
        this.Add(VPane);

        Child.ShowAll();
        //Build();

    }

    private GardenDataEntry SelectedEntry;

    private void SelectEntry(GardenDataEntry entry)
    {
        string type = "";
        if (entry is PlantFamily)
            type = "family";
        if (entry is Plant)
            type = "plant";
        if (entry is PlantVariety)
            type = "variety";

        EditButton.Label = "Edit " + type + " '" + entry.Name + "'";
        //SelectedEntry = entry;

        SelectedEntry = entry;

    }

    private ComboBox PopulateVarieties(Plant plant)
    {
        ComboBox varietyBox = new ComboBox(empty);
        varietyIDs = new List<string>();
        foreach (KeyValuePair<string, PlantVariety> pair in plant.Varieties)
        {
            varietyBox.AppendText(pair.Value.Name);
            varietyIDs.Add(pair.Key);
        }
        varietyBox.AppendText("<add variety>");

        varietyBox.TooltipText = "Select variety";

        varietyBox.Changed += (object sender, System.EventArgs e) =>
        {
            if (varietyBox.Active < varietyIDs.Count)
            {
                SelectEntry(plant.Varieties[varietyIDs[varietyBox.Active]]);
            }
            else //selected add action
            {
                new EditPlantVarietyWindow(plant).ShowAll();
            }
        };

        return varietyBox;
    }

    private ComboBox PopulatePlants(PlantFamily family)
    {
        ComboBox plantBox = new ComboBox(empty);
        plantIDs = new List<string>();
        foreach (KeyValuePair<string, Plant> pair in family.Plants)
        {
            plantBox.AppendText(pair.Value.Name);
            plantIDs.Add(pair.Key);
        }

        plantBox.AppendText("<add plant>");

        plantBox.TooltipText = "Select plant";
        plantBox.AddEvents((int)Gdk.EventMask.AllEventsMask);

        plantBox.Changed += (object sender, System.EventArgs e) =>
        {
            if (plantBox.Active < plantIDs.Count)
            {
                PlantVarietySelector.Remove(VarietyBox);
                VarietyBox = PopulateVarieties(family.Plants[plantIDs[plantBox.Active]]);
                PlantVarietySelector.Add2(VarietyBox);
                Build();
            }
            else //selected add action
            {
                System.Console.WriteLine("TODO: Add plant"); //TODO
            }
        };

        return plantBox;
    }

    private ComboBox PopulateFamilies(GardenData data)
    {
        ComboBox familyBox = new ComboBox(empty);
        familyIDs = new List<string>();
        foreach (KeyValuePair<string, PlantFamily> pair in GardenData.LoadedData.Families)
        {
            familyBox.AppendText(pair.Value.Name);
            familyIDs.Add(pair.Key);
        }

        familyBox.AppendText("<add family>");

        familyBox.TooltipText = "Select plant family";
        familyBox.AddEvents((int)Gdk.EventMask.AllEventsMask);

        familyBox.Changed += (object sender, System.EventArgs e) =>
        {
            if (familyBox.Active < familyIDs.Count)// && FamilyBox.Active < PlantBoxes.Count)
            {
                FamilyPlantVarietySelector.Remove(PlantVarietySelector);
                PlantVarietySelector = new VPaned();
                PlantVarietySelector.Add1(PopulatePlants(GardenData.LoadedData.Families[familyIDs[familyBox.Active]]));
                FamilyPlantVarietySelector.Add2(PlantVarietySelector);
                Build();
            }
            else //selected add action
            {
                System.Console.WriteLine("TODO: Add family"); //TODO
            }
        };

        return familyBox;
    }

    public void RepopulateGrowables()
    {
        FamilyPlantVarietySelector.Add1(PopulateFamilies(GardenData.LoadedData));
    }

    protected void OnDeleteEvent(object sender, DeleteEventArgs a)
    {
        Application.Quit();
        a.RetVal = true;
    }
}
