using System.Collections.Generic;
using GardenPlanner;
using GardenPlanner.Garden;

using Gtk;

public partial class MainWindow : Window
{
    MenuBar MenuBar = new MainWindowMenuBar();
    Notebook GardenBedBook = new Notebook();
    HPaned GraphicsSidebarHPaned = new HPaned();
    VPaned TopPanedToolboxVPaned = new VPaned();
    VPaned PlantVarietySelector = null;
    VPaned FamilyPlantVarietySelector = new VPaned();
    VPaned PlantSideVPaned = new VPaned();
    Box PlantBox = new VBox();
    ComboBox VarietyBox = null;
    HButtonBox ToolBox = new HButtonBox();
    Button PlantEditButton = new Button("Edit");
    Button PlantInfoButton = new Button("Info");
    public Button PlantAddButton = new Button("Add");

    SpinButton ZoomButton = new SpinButton(0.1, 10, 0.1);
    Button AreaDeleteButton = new Button("Delete Area");
    public ToggleButton AreaNewButton = new ToggleButton("New Area");
    ComboBox AreaTypeComboBox = new ComboBox(new string[] {"Garden", "Planting", "MethodArea"});
    Button AreaCancelButton = new Button("Cancel");
    bool AreaCancelButtonClicked = false;

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

        GraphicsSidebarHPaned.Add1(GardenBedBook);
        GraphicsSidebarHPaned.Add2(PlantSideVPaned);

        //TODO RepopulateGrowables(); //adds growables selection to top of PlantSideVPaned
        ComboBox families = PopulateFamilies(GardenData.LoadedData);
        FamilyPlantVarietySelector.Add1(families);
        PlantSideVPaned.Add1(FamilyPlantVarietySelector);

        FamilyPlantVarietySelector.SetSizeRequest(100, 400);
        PlantSideVPaned.Add2(PlantBox);

        TopPanedToolboxVPaned.Add1(GraphicsSidebarHPaned);
        TopPanedToolboxVPaned.Add2(ToolBox);

        Frame frame;

        PlantBox.Add(PlantAddButton);
        PlantAddButton.Sensitive = false;
        PlantAddButton.Clicked += (object sender, System.EventArgs e) =>
        {
            GardenDrawingArea area = GardenDrawingArea.ActiveInstance;
            if (area.SelectedArea is Planting planting)
            {
                if (SelectedEntry is PlantVariety variety)
                {
                    planting.AddVariety(variety); //TODO count
                }
            }
        };

        PlantBox.Add(PlantInfoButton);
        PlantInfoButton.Clicked += (object sender, System.EventArgs e) =>
        {
            if (SelectedEntry is PlantFamily family)
            { }
            else if (SelectedEntry is Plant plant)
            { }
            else if (SelectedEntry is PlantVariety variety)
            {
                InfoPlantVarietyWindow.ShowWindow(variety, false);
            }
        };

        PlantBox.Add(PlantEditButton);

        //TODO add more actions for other widgets or save selected entry so other widgets know what is currently selected
        PlantEditButton.Clicked += (object sender, System.EventArgs e) =>
        {
            if (SelectedEntry is PlantFamily family)
            { }
            else if (SelectedEntry is Plant plant)
            { }
            else if (SelectedEntry is PlantVariety variety)
            {
                EditPlantVarietyWindow.ShowWindow(variety);
            }
        };


        ZoomButton.Value = GardenDrawingArea.Zoom;
        ZoomButton.Events = Gdk.EventMask.ButtonPressMask | Gdk.EventMask.ButtonReleaseMask | Gdk.EventMask.KeyPressMask | Gdk.EventMask.KeyReleaseMask;
        ZoomButton.TooltipText = "Zoom";

        ToolBox.Add(AreaNewButton);

        AreaNewButton.Clicked += (object sender, System.EventArgs e) =>
        {
            if (AreaCancelButtonClicked)
            {
                AreaCancelButtonClicked = false;
                AreaCancelButton.Sensitive = false;
                if (GardenDrawingArea.ActiveInstance != null)
                {
                    GardenDrawingArea.ActiveInstance.UndoSelection();
                }

            }
            else if (!AreaNewButton.Active)//deactivated
            {
                AreaCancelButton.Sensitive = false;
                if (GardenDrawingArea.ActiveInstance != null)
                {
                    List<GardenPoint> points = GardenDrawingArea.ActiveInstance.NewPoints;
                    switch (AreaTypeComboBox.Active)
                    {
                        case 0://garden
                            System.Console.WriteLine("new garden");//TODO add garden
                            break;
                        case 1://planting
                            Planting planting = new Planting("", "");//TODO name, descr
                            planting.Shape.AddPoints(points);
                            planting.Shape.FinishPoints();
                            GardenDrawingArea.ActiveInstance.Garden.AddPlanting("some_key", planting);//TODO key
                            break;
                        case 2://method area
                            //TODO add method area
                            break;
                    }
                }
            }
            else //activated
            {
                AreaCancelButton.Sensitive = true;
            }


            if (GardenDrawingArea.ActiveInstance != null)
            {
                GardenDrawingArea.ActiveInstance.NewPoints.Clear();
                GardenDrawingArea.ActiveInstance.Draw();
            }
        };

        ToolBox.Add(AreaCancelButton);
        AreaCancelButton.Sensitive = false;
        AreaCancelButton.Clicked += (object sender, System.EventArgs e) =>
        {
            AreaCancelButtonClicked = true;
            AreaCancelButton.Sensitive = false;
            AreaNewButton.Active = false;
        };

        ToolBox.Add(AreaTypeComboBox);
        AreaTypeComboBox.Active = 0;
        AreaTypeComboBox.Changed += (object sender, System.EventArgs e) =>
        {
            switch (AreaTypeComboBox.Active)
            {
                case 0:
                    AreaNewButton.Label = "New Garden";
                    break;
                case 1:
                    AreaNewButton.Label = "New Planting";
                    break;
                case 2:
                    AreaNewButton.Label = "New Method Area";
                    break;
            }
        };

        ToolBox.Add(AreaDeleteButton);

        AreaDeleteButton.Clicked += (object sender, System.EventArgs e) =>
        {
            GardenDrawingArea gardenDrawingArea = GardenDrawingArea.ActiveInstance;

            if (gardenDrawingArea == null)
                return;

            if (gardenDrawingArea.Garden != null && (gardenDrawingArea.SelectedArea == null || gardenDrawingArea.SelectedArea is Garden))
            {
                var garden = gardenDrawingArea.Garden;

                GardenData.LoadedData.Gardens.Remove(garden.ID);
                GardenBedBook.Remove(GardenBedBook.GetNthPage(GardenBedBook.CurrentPage));
                //System.Console.WriteLine("delete garden");
                GardenDrawingArea.ActiveInstance = null;
                gardenDrawingArea.UndoSelection();
                SelectGardenEntry(null);
                if (GardenBedBook.Page >= 0 && GardenBedBook.GetNthPage(GardenBedBook.Page) is GardenDrawingArea drawingArea)
                {
                    GardenDrawingArea.ActiveInstance = drawingArea;
                    drawingArea.Draw();
                }
            }
            else if (gardenDrawingArea.SelectedArea is Planting planting)
            {
                gardenDrawingArea.Garden.RemovePlanting(planting);
                gardenDrawingArea.UndoSelection();
                gardenDrawingArea.Draw();
            }
            else if (gardenDrawingArea.SelectedArea is GardenArea area)
            {
                gardenDrawingArea.Garden.RemoveMethodArea(area);
                gardenDrawingArea.UndoSelection();
                gardenDrawingArea.Draw();
            }
        };

        frame = new Frame("Zoom");
        frame.Add(ZoomButton);
        ToolBox.Add(frame);

        //FamilyPlantVarietySelector.Add1(PopulateFamilies(GardenData.LoadedData)); 
        //RepopulateGrowables();

        foreach (KeyValuePair<string, Garden> pair in GardenData.LoadedData.Gardens)
        {
            string key = pair.Key;
            Garden garden = pair.Value;

            GardenDrawingArea gda = new GardenDrawingArea(garden, ZoomButton);

            GardenBedBook.AppendPage(gda, new Label(garden.Name));

            //set first instance in the list to active instance
            if (GardenDrawingArea.ActiveInstance == null)
                GardenDrawingArea.ActiveInstance = gda;
        }

        GardenBedBook.SwitchPage += (object o, SwitchPageArgs args) =>
        {
            try
            {
                if (GardenBedBook.GetNthPage(checked((int)args.PageNum)) is GardenDrawingArea drawingArea)
                {
                    GardenDrawingArea.ActiveInstance = drawingArea;
                    SelectGardenEntry(drawingArea.Garden);
                    System.Console.WriteLine("selected " + drawingArea.Garden.Name);
                };
            }
            catch (System.OverflowException)
            {
                System.Console.WriteLine("Too many garden bed book pages");
            }
        };

        //this.Add(MenuBar);
        this.Add(TopPanedToolboxVPaned);

        Child.ShowAll();
        //Build();

    }

    private GardenDataEntry SelectedEntry;

    public void SelectGardenEntry(GardenArea area)
    {
        string type = "";
        if (area is Garden)
            type = "garden";
        else if (area is Planting)
            type = "planting";
        else
            type = "method area";

        if (area != null)
            AreaDeleteButton.Label = "Delete " + type + " '" + area.Name + "'";
        else
            AreaDeleteButton.Label = "Delete";
    }

    private void SelectPlantEntry(GardenDataEntry entry)
    {
        string type = "";
        if (entry is PlantFamily)
            type = "family";
        if (entry is Plant)
            type = "plant";
        if (entry is PlantVariety)
            type = "variety";

        PlantEditButton.Label = "Edit " + type + " '" + entry.Name + "'";
        PlantInfoButton.Label = "Info for " + type + " '" + entry.Name + "'";
        PlantAddButton.Label = "Add " + type + " '" + entry.Name + "'";

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
                SelectPlantEntry(plant.Varieties[varietyIDs[varietyBox.Active]]);
            }
            else //selected add action
            {
                EditPlantVarietyWindow.ShowWindow(plant);
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
                //if (VarietyBox != null)
                //    PlantVarietySelector.Remove(VarietyBox);

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
                if (PlantVarietySelector != null)
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

    //Widget OldFamilies;
    public void RepopulateGrowables()
    {
        //if (FamilyPlantVarietySelector != null)
        //    PlantSideVPaned.Remove(FamilyPlantVarietySelector);

    }

    protected void OnDeleteEvent(object sender, DeleteEventArgs a)
    {
        Application.Quit();
        a.RetVal = true;
    }
}
