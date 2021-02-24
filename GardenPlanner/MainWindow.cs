// Copyright (c) 2020 Jahangmar
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <https://www.gnu.org/licenses/>.
//

using System.Collections.Generic;
using GardenPlanner;
using GardenPlanner.Garden;

using Gtk;

public partial class MainWindow : Window
{
    GardenPlannerSettings settings = GardenPlannerSettings.GetSettings();
    MainWindowMenuBar MenuBar = new MainWindowMenuBar();
    VBox TopVBox = new VBox();
    Notebook GardenBedBook = new Notebook();
    HPaned GraphicsSidebarHPaned = new HPaned();
    VPaned TopPanedToolboxVPaned = new VPaned();

    VPaned FamilyPlantVarietySelector = new VPaned();
    VPaned PlantSideVPaned = new VPaned();
    Box PlantBox = new VBox();

    VPaned PlantAreaInfoVPaned = new VPaned();
    InfoView AreaInfo = new InfoView();

    HButtonBox ToolBox = new HButtonBox();
    Button PlantEditButton = new Button("Edit");
    Button PlantInfoButton = new Button("Info");
    public Button PlantAddButton = new Button("");

    SpinButton ZoomButton = new SpinButton(0.1, 10, 0.1);
    public Button AreaDeleteButton = new Button("Delete Area");
    public Button AreaEditButton = new Button("Edit Area");
    public ToggleButton AreaNewButton = new ToggleButton("New Area");
    ComboBox AreaTypeComboBox = new ComboBox(new string[] {"Garden", "Planting", "MethodArea"});
    Button AreaCancelButton = new Button("Cancel");
    bool AreaCancelButtonClicked = false;

    Frame DateFrame = new Frame();
    SpinButton yearButton = new SpinButton(2000, 2100,1), monthButton = new SpinButton(1,12,1);

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
        ToolBox.Add(AreaEditButton);
        ToolBox.Add(AreaDeleteButton);

        frame = new Frame("Zoom");
        frame.Add(ZoomButton);
        ToolBox.Add(frame);

        frame = new Frame("Date");
        VButtonBox buttonBox = new VButtonBox();
        //yearButton = new SpinButton(GardenData.GetFirstYear(), GardenData.GetLastYear(), 1);
        yearButton = new SpinButton(settings.MinYear, settings.MaxYear, 1);
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
        PlantAddButton.Clicked += (sender, e) =>
        {
            GardenDrawingArea area = GardenDrawingArea.ActiveInstance;
            if (area.SelectedArea is Planting planting && SelectedEntry is PlantVariety variety)
            {
                EditPlantingInfoWindow.ShowPlantingInfoWindow((PlantingInfo plantingInfo) =>
                {
                    planting.AddVariety(variety, plantingInfo);
                    GardenDrawingArea.ActiveInstance.Draw();
                    ShowAreaSelectionInfo(area.SelectedArea);
                }, planting, variety.Name);
            };
        };
       
        PlantInfoButton.Clicked += (object sender, System.EventArgs e) =>
        {
            if (SelectedEntry is PlantFamily family)
            {
                InfoPlantFamilyWindow.ShowWindow(family, false);
            }
            else if (SelectedEntry is Plant plant)
            {
                InfoPlantWindow.ShowWindow(plant, false);
            }
            else if (SelectedEntry is PlantVariety variety)
            {
                InfoPlantVarietyWindow.ShowWindow(variety, false);
            }
        };


        PlantEditButton.Clicked += (object sender, System.EventArgs e) =>
        {
            if (SelectedEntry is PlantFamily family)
            {
                EditPlantFamilyWindow.ShowWindow(family);
            }
            else if (SelectedEntry is Plant plant)
            {
                EditPlantWindow.ShowWindow(plant);
            }
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
                    GardenDrawingArea.ActiveInstance.NewPoints.Clear();
                    GardenDrawingArea.ActiveInstance.Draw();
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
                            if (GardenDrawingArea.ActiveInstance.Garden.Shape.GetPoints().Count == 0)
                            {
                                GardenDrawingArea.ActiveInstance.Garden.Shape.AddPoints(points);
                                GardenDrawingArea.ActiveInstance.Garden.Shape.FinishPoints();
                                GardenDrawingArea.ActiveInstance.NewPoints.Clear();
                                GardenDrawingArea.ActiveInstance.Draw();
                            }
                            break;
                        case 1://planting
                            PlantingCreationDialog.ShowPlantingCreationDialog(new List<GardenPoint>(points), (Planting planting) =>
                            {
                                GardenDrawingArea.ActiveInstance.NewPoints.Clear();
                                GardenDrawingArea.ActiveInstance.Draw();
                                GardenDrawingArea.ActiveInstance.Garden.AddPlanting(GardenData.GenID(planting.Name), planting);
                            });
                                    

                            break;
                        case 2://method area
                            GardenAreaCreationDialog.ShowGardenAreaCreationDialog(new List<GardenPoint>(points), (GardenArea area) => {
                                GardenDrawingArea.ActiveInstance.NewPoints.Clear();
                                GardenDrawingArea.ActiveInstance.Draw();
                                GardenDrawingArea.ActiveInstance.Garden.AddMethodArea(GardenData.GenID(area.Name), area);
                            });

                            break;
                    }
                }

            }
            else //activated
            {
                if (AreaTypeComboBox.Active == 0) //garden
                {
                    if (GardenDrawingArea.ActiveInstance == null || GardenDrawingArea.ActiveInstance.Garden.Shape.GetPoints().Count > 0)
                    {
                        AreaNewButton.Active = false;
                        GardenCreationDialog.ShowGardenCreationDialog(new List<GardenPoint>(), ((Garden garden) =>
                        {
                            GardenData.LoadedData.AddGarden(GardenData.GenID(garden.Name), garden);
                            this.ResetForNewData();
                            GardenBedBook.Page = GardenBedBook.NPages - 1;
                            AreaNewButton.Active = true;
                        }));
                            

                    }
                }

                AreaCancelButton.Sensitive = true;
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

        AreaEditButton.Clicked += (object sender, System.EventArgs e) =>
        {
            GardenDrawingArea gardenDrawingArea = GardenDrawingArea.ActiveInstance;

            if (gardenDrawingArea == null || gardenDrawingArea.Garden == null || gardenDrawingArea.SelectedArea == null)
                return;

            if (gardenDrawingArea.SelectedArea is Planting planting)
                PlantingCreationDialog.ShowPlantingEditDialog(planting);
            else if (gardenDrawingArea.SelectedArea is Garden)
                GardenAreaCreationDialog.ShowGardenAreaEditDialog(gardenDrawingArea.SelectedArea);
            else
                GardenAreaCreationDialog.ShowGardenAreaEditDialog(gardenDrawingArea.SelectedArea);
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
                    GardenData.unsaved = true;
                }
                else if (gardenDrawingArea.SelectedArea is Planting planting)
                {
                    gardenDrawingArea.Garden.RemovePlanting(planting);
                    gardenDrawingArea.UndoSelection();
                    SelectGardenEntry(null);
                    gardenDrawingArea.Draw();
                    GardenData.unsaved = true;
                }
                else if (gardenDrawingArea.SelectedArea is GardenArea area)
                {
                    gardenDrawingArea.Garden.RemoveMethodArea(area);
                    gardenDrawingArea.UndoSelection();
                    SelectGardenEntry(null);
                    gardenDrawingArea.Draw();
                    GardenData.unsaved = true;
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
            if (gardenDrawingArea == null)
                return;

            if (gardenDrawingArea.SelectedArea != null && (!gardenDrawingArea.SelectedArea.CheckDate(GetYear(), GetMonth()) || !gardenDrawingArea.Garden.CheckDate(GetYear(), GetMonth())))
            {
                gardenDrawingArea.UndoSelection();
            }
            gardenDrawingArea.Draw();
            ShowAreaSelectionInfo(gardenDrawingArea.SelectedArea);
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
            GardenPlannerSettings.Save();
            Application.Quit();
        };
    }

    public void ResetForNewData()
    {
        SelectedEntry = null;

        PlantAddButton.Sensitive = false;
        AreaEditButton.Sensitive = false;

        GardenDrawingArea.ActiveInstance = null;

        yearButton.Value = System.DateTime.Now.Year;
        monthButton.Value = System.DateTime.Now.Month;

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
                };
            }
            catch (System.OverflowException)
            {
                System.Console.WriteLine("Too many garden bed book pages");
            }
        };
    }

    public void ReloadFamilies()
    {
        SelectedEntry = null;
        PlantAddButton.Sensitive = false;
        SelectPlantEntry();
        RemoveAllChildren(FamilyPlantVarietySelector);
        FamilyTreeView familyTreeView = new FamilyTreeView(GardenData.LoadedData)
        {
            SelectedGardenDataEntry = SelectPlantEntry,
            NewFamily = EditPlantFamilyWindow.ShowWindow,
            NewPlant = EditPlantWindow.ShowWindow,
            NewVariety = EditPlantVarietyWindow.ShowWindow
        };
        FamilyPlantVarietySelector.Add1(familyTreeView);
        ShowAreaSelectionInfo(GardenDrawingArea.ActiveInstance?.SelectedArea);
        ShowAll();
    }

private void SelectPlantEntry()
{
    PlantAddButton.Label = "";
    PlantEditButton.Label = "";
    PlantInfoButton.Label = "";
    PlantEditButton.Sensitive = false;
    PlantInfoButton.Sensitive = false;
}

private void SelectPlantEntry(GardenDataEntry entry)
{
    if (entry == null)
    {
        SelectPlantEntry();
        return;
    }

    string type = "";
    if (entry is PlantFamily)
    {
        type = "family";
        PlantAddButton.Sensitive = false;
        PlantAddButton.Label = "";
    }
    if (entry is Plant)
    {
        type = "plant";
        PlantAddButton.Sensitive = false;
        PlantAddButton.Label = "";
    }
    if (entry is PlantVariety)
    {
        type = "variety";
        PlantAddButton.Sensitive = GardenDrawingArea.ActiveInstance?.SelectedArea is Planting;
        PlantAddButton.Label = "Add " + type + " '" + entry.Name + "' to planting";
    }

    PlantEditButton.Label = "Edit " + type + " '" + entry.Name + "'";
    PlantInfoButton.Label = "Info for " + type + " '" + entry.Name + "'";
    PlantEditButton.Sensitive = true;
    PlantInfoButton.Sensitive = true;

    SelectedEntry = entry;
}

private void RemoveAllChildren(Container w)
    {
        foreach (Widget c in w.AllChildren)
            w.Remove(c);
    }

    public void TryToClose()
    {
        if (GardenData.unsaved)
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
                if (!GardenData.unsaved)
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
        if (area == null)
        {
            ShowEmptyAreaSelectionInfo();
            return;
        }

        AreaInfo.Buffer.Clear();

        AreaInfo.AddEntry(area.Name, AreaInfo.tag_headline);
        AreaInfo.AddEntry(area.Description, AreaInfo.tag_italic);
        AreaInfo.AddEntry("Created: "+area.created.Month+"/"+area.created.Year);
        AreaInfo.AddEntry("Removed: " + area.removed.Month + "/" + area.removed.Year);
        AreaInfo.AddEntry($"Size: {area.AreaSize() / 10000f}m²");

        if (area is Planting planting)
        {
            AreaInfo.AddEntry("Varieties:");
            foreach (KeyValuePair<VarietyKeySeq, PlantingInfo> pair in planting.Varieties)
            {
                VarietyKeySeq seq = pair.Key;
                int count = pair.Value.Count;
                PlantVariety variety = GardenData.LoadedData.GetVariety(seq);
                Plant plant = GardenData.LoadedData.GetPlant(seq.FamilyKey, seq.PlantKey);
                AreaInfo.AddEntry(variety.Name + " (" + plant.Name + "): " + count);
            }
        }
        AreaInfo.ApplyTags();
    }

    public void ShowEmptyAreaSelectionInfo()
    {
        AreaInfo.Buffer.Clear();

        AreaInfo.AddEntry("TODO LIST", AreaInfo.tag_headline);

        int syear = GetYear();
        int smonth = GetMonth();
        int eyear = GetYear();
        int emonth = GetMonth();
        DateRange range = new DateRange(syear, smonth, eyear, emonth);
        foreach (Garden garden in GardenData.LoadedData.Gardens.Values)
        {
            if (garden.CheckDate(syear, smonth) || garden.CheckDate(eyear, emonth))
            {
                List<string> strings = garden.GetTodoList(range);
                if (strings.Count > 0)
                    AreaInfo.AddEntry("Garden '" + garden.Name + "':", AreaInfo.tag_bold);
                foreach (string s in strings)
                {
                    if (s.StartsWith("#"))
                        AreaInfo.AddEntry("\t" + s.Substring(1), AreaInfo.tag_strikethrough);
                    else
                        AreaInfo.AddEntry("\t" + s, AreaInfo.tag_italic);
                }
            }
        }
        AreaInfo.ApplyTags();
    }

    public int GetYear() => yearButton.ValueAsInt;

    public int GetMonth() => monthButton.ValueAsInt;

    public GardenDataEntry SelectedEntry;

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

    protected void OnDeleteEvent(object sender, DeleteEventArgs a)
    {
        Application.Quit();
        a.RetVal = true;
    }
}
