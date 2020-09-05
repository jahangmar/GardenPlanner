using System.Collections.Generic;
using GardenPlanner;
using GardenPlanner.Garden;

using Gtk;

public partial class MainWindow : Window
{
    MainWindowMenuBar MenuBar = new MainWindowMenuBar();
    VBox TopVBox = new VBox();
    Notebook GardenBedBook = new Notebook();
    HPaned GraphicsSidebarHPaned = new HPaned();
    VPaned TopPanedToolboxVPaned = new VPaned();
    VPaned PlantVarietySelector = null;
    VPaned FamilyPlantVarietySelector = new VPaned();
    VPaned PlantSideVPaned = new VPaned();
    Box PlantBox = new VBox();

    VPaned PlantAreaInfoVPaned = new VPaned();
    InfoView AreaInfo = new InfoView();

    ComboBox VarietyBox = null;
    HButtonBox ToolBox = new HButtonBox();
    Button PlantEditButton = new Button("Edit");
    Button PlantInfoButton = new Button("Info");
    public Button PlantAddButton = new Button("Add");

    SpinButton ZoomButton = new SpinButton(0.1, 10, 0.1);
    public Button AreaDeleteButton = new Button("Delete Area");
    public ToggleButton AreaNewButton = new ToggleButton("New Area");
    ComboBox AreaTypeComboBox = new ComboBox(new string[] {"Garden", "Planting", "MethodArea"});
    Button AreaCancelButton = new Button("Cancel");
    bool AreaCancelButtonClicked = false;

    Frame DateFrame = new Frame();
    SpinButton yearButton = new SpinButton(2020,2030,1), monthButton = new SpinButton(1,12,1);

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
        GardenData.LoadedData = GardenData.LoadedData = new GardenData("new project");
        Title = "Garden project '" + GardenData.LoadedData.Name + "'";

        //GardenData.LoadedData = GardenPlanner.MainClass.TestData();





        PlantSideVPaned.Add1(FamilyPlantVarietySelector);


        PlantAreaInfoVPaned.Add1(PlantBox);
        PlantAreaInfoVPaned.Add2(AreaInfo);

        PlantSideVPaned.Add2(PlantAreaInfoVPaned);

        TopPanedToolboxVPaned.Add1(GraphicsSidebarHPaned);
        TopPanedToolboxVPaned.Add2(ToolBox);

        Frame frame;

        PlantBox.Add(PlantAddButton);
        PlantBox.Add(PlantInfoButton);
        PlantBox.Add(PlantEditButton);

        ToolBox.Add(AreaNewButton);
        ToolBox.Add(AreaCancelButton);
        ToolBox.Add(AreaTypeComboBox);
        ToolBox.Add(AreaDeleteButton);

        frame = new Frame("Zoom");
        frame.Add(ZoomButton);
        ToolBox.Add(frame);

        frame = new Frame("Date");
        VButtonBox buttonBox = new VButtonBox();
        yearButton = new SpinButton(GardenData.GetFirstYear(), GardenData.GetLastYear(), 1);
        buttonBox.Add(yearButton);
        buttonBox.Add(monthButton);

        frame.Add(buttonBox);
        ToolBox.Add(frame);

        foreach (Widget w in TopVBox.Children)
            TopVBox.Remove(w);
        TopVBox.Add(MenuBar);
        TopVBox.Add(TopPanedToolboxVPaned);

        if (this.Child == null)
            this.Add(TopVBox);






        ResetForNewData();

        FamilyPlantVarietySelector.SetSizeRequest(100, 400);

        AreaInfo.WrapMode = WrapMode.Word;
        AreaInfo.Editable = false;


        PlantAddButton.Sensitive = false;
        PlantAddButton.Clicked += (object sender, System.EventArgs e) =>
        {
            GardenDrawingArea area = GardenDrawingArea.ActiveInstance;
            if (area.SelectedArea is Planting planting && SelectedEntry is PlantVariety variety)
            {
                NumberInputWindow.ShowWindow("Adding "+variety.Name+"...", "Select the amount", 1, 500, (int res) => {

                    {
                        planting.AddVariety(variety, res);
                        GardenDrawingArea.ActiveInstance.Draw();
                    }
                });


            }
        };


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
//                            GardenCreationDialog.ShowGardenCreationDialog(new List<GardenPoint>(points), ((Garden garden) =>
//                                    ???TODO
                            break;
                        case 1://planting
                            PlantingCreationDialog.ShowPlantingCreationDialog(new List<GardenPoint>(points), (Planting planting) =>
                                    GardenDrawingArea.ActiveInstance.Garden.AddPlanting(GardenData.GenID(planting.Name), planting));

                            break;
                        case 2://method area
                            GardenAreaCreationDialog.ShowGardenAreaCreationDialog(new List<GardenPoint>(points), (GardenArea area) =>
                                    GardenDrawingArea.ActiveInstance.Garden.AddMethodArea(GardenData.GenID(area.Name), area));
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


        AreaCancelButton.Sensitive = false;
        AreaCancelButton.Clicked += (object sender, System.EventArgs e) =>
        {
            AreaCancelButtonClicked = true;
            AreaCancelButton.Sensitive = false;
            AreaNewButton.Active = false;
        };


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



        AreaDeleteButton.Clicked += (object sender, System.EventArgs e) =>
        {
            GardenDrawingArea gardenDrawingArea = GardenDrawingArea.ActiveInstance;

            if (gardenDrawingArea == null || gardenDrawingArea.Garden == null)
                return;

            string name = gardenDrawingArea.SelectedArea != null ? gardenDrawingArea.SelectedArea.Name : gardenDrawingArea.Garden.Name;

            Dialog dialog = new MessageDialog(this, DialogFlags.Modal, MessageType.Warning, ButtonsType.OkCancel, "Do you want to delete '"+name+"'?", new { });

            int response = dialog.Run();
            //System.Console.WriteLine("response " + result);
            if (response == (int)ResponseType.Cancel)
            {

            }
            else if (response == (int)ResponseType.Ok)
            {

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
                    SelectGardenEntry(null);
                    gardenDrawingArea.Draw();
                }
                else if (gardenDrawingArea.SelectedArea is GardenArea area)
                {
                    gardenDrawingArea.Garden.RemoveMethodArea(area);
                    gardenDrawingArea.UndoSelection();
                    SelectGardenEntry(null);
                    gardenDrawingArea.Draw();
                }
            }
            dialog.Destroy();

        };



        int yearValue = yearButton.ValueAsInt;
        yearButton.ValueChanged += (sender, e) =>
        {
            if (yearButton.ValueAsInt > yearValue)
                monthButton.Value = 1;
            else
                monthButton.Value = 12;
            yearValue = yearButton.ValueAsInt;
            DateChanged();
        };
        monthButton.Changed += (sender, e) =>
        {
            DateChanged();
        };

        void DateChanged()
        {
            GardenDrawingArea gardenDrawingArea = GardenDrawingArea.ActiveInstance;
            if (gardenDrawingArea.SelectedArea != null && !gardenDrawingArea.SelectedArea.CheckDate(yearButton.ValueAsInt, monthButton.ValueAsInt))
            {
                gardenDrawingArea.UndoSelection();
            }
            gardenDrawingArea.Draw();
        }

        //FamilyPlantVarietySelector.Add1(PopulateFamilies(GardenData.LoadedData)); 
        //RepopulateGrowables();



        //Build();
        this.DeleteEvent += (object o, DeleteEventArgs args) =>
        {
            TryToClose();
            args.RetVal = true;
        };

        this.Destroyed += (sender, e) =>
        {
            Application.Quit();
        };
    }

    public void ResetForNewData()
    {/*
        MenuBar = new MainWindowMenuBar();
        TopVBox = new VBox();
        GardenBedBook = new Notebook();
        GraphicsSidebarHPaned = new HPaned();
        TopPanedToolboxVPaned = new VPaned();
        PlantVarietySelector = null;
        FamilyPlantVarietySelector = new VPaned();
        PlantSideVPaned = new VPaned();
        PlantBox = new VBox();

        PlantAreaInfoVPaned = new VPaned();
        AreaInfo = new InfoView();

        VarietyBox = null;
        ToolBox = new HButtonBox();
        PlantEditButton = new Button("Edit");
        PlantInfoButton = new Button("Info");
        PlantAddButton = new Button("Add");

        ZoomButton = new SpinButton(0.1, 10, 0.1);
        AreaDeleteButton = new Button("Delete Area");
        AreaNewButton = new ToggleButton("New Area");
        AreaTypeComboBox = new ComboBox(new string[] { "Garden", "Planting", "MethodArea" });
        AreaCancelButton = new Button("Cancel");
        AreaCancelButtonClicked = false;

        DateFrame = new Frame();
        yearButton = new SpinButton(2020, 2030, 1);
        monthButton = new SpinButton(1, 12, 1);
        */
        ReloadFamilies();

        RemoveAllChildren(GraphicsSidebarHPaned);

        GraphicsSidebarHPaned.Add1(GardenBedBook);
        GraphicsSidebarHPaned.Add2(PlantSideVPaned);



        while (GardenBedBook.NPages > 0)
            GardenBedBook.RemovePage(0);

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
        Child.ShowAll();
        //Build();

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
    }

    private void ReloadFamilies()
    {
        ComboBox families = PopulateFamilies(GardenData.LoadedData);
        RemoveAllChildren(FamilyPlantVarietySelector);
        FamilyPlantVarietySelector.Add1(families);
        ShowAll();
    }

    private void RemoveAllChildren(Container w)
    {
        foreach (Widget c in w.AllChildren)
            w.Remove(c);
    }

    public void TryToClose()
    {
        if (GardenData.LoadedData.unsaved)
        {
            Dialog dialog = new MessageDialog(this, DialogFlags.Modal, MessageType.Warning, ButtonsType.None, "There are unsaved changes. Really quit?", new { });
            dialog.AddButton("Cancel", ResponseType.Cancel);
            dialog.AddButton("Close", ResponseType.Close);
            dialog.AddButton("Save and Close", ResponseType.Apply);
            int response = dialog.Run();
            //System.Console.WriteLine("response " + result);
            if (response == (int)ResponseType.Cancel)
            {
                //do nothing
            }
            else if (response == (int)ResponseType.Close)
            {
                this.Destroy();//close main window (also quits app)
            }
            else if (response == (int)ResponseType.Apply)
            {
                MenuBar.Save();
                this.Destroy();
            }
            dialog.Destroy();
        }
        else
        {
            this.Destroy();
        }
    }

    public void ShowAreaSelectionInfo(GardenArea area)
    {
        AreaInfo.Buffer.Clear();

        AreaInfo.AddEntry(area.Name, AreaInfo.headline);
        AreaInfo.AddEntry(area.Description, AreaInfo.italic);

        //TODO info for all 3 types
        if (area is Garden)
        {

        }
        AreaInfo.ApplyTags();
    }

    public void ShowEmptyAreaSelectionInfo()
    {
        AreaInfo.Buffer.Clear();
    }

    public int GetYear() => yearButton.ValueAsInt;

    public int GetMonth() => monthButton.ValueAsInt;

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
        {
            AreaDeleteButton.Label = "Delete " + type + " '" + area.Name + "'";
            ShowAreaSelectionInfo(area);
        }
        else
        {
            AreaDeleteButton.Label = "Delete";
            ShowEmptyAreaSelectionInfo();
        }
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
                ReloadFamilies();
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
            System.Console.WriteLine("found family: " + pair.Value.Name);
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
