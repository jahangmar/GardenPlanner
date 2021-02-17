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

using System;
using System.Collections.Generic;
using GLib;
using Gtk;

namespace GardenPlanner
{
    public class MainWindowMenuBar : MenuBar
    {
        MenuItem FileItem = new MenuItem("File");
        Menu FileMenu = new Menu();
        MenuItem MenuItemNew = new MenuItem("New");
        MenuItem MenuItemLoad = new MenuItem("Load");
        MenuItem MenuItemSave = new MenuItem("Save");
        MenuItem MenuItemImportFamilies = new MenuItem("Import plant families");
        MenuItem MenuItemClose = new MenuItem("Close");

        MenuItem SettingsItem = new MenuItem("Settings");
        Menu SettingsMenu = new Menu();
        MenuItem LanguageItem = new MenuItem("Language");
        Menu LanguageMenu = new Menu();
        CheckMenuItem ShowAreaImagesItem = new CheckMenuItem("Show area images");
        CheckMenuItem ShowPlantNames = new CheckMenuItem("Show plant names");
        CheckMenuItem ShowVarietyNames = new CheckMenuItem("Show variety names");

        MenuItem HelpItem = new MenuItem("Help");
        Menu HelpMenu = new Menu();
        //MenuItem MenuItemGetHelp = new MenuItem("")
        MenuItem MenuItemAbout = new MenuItem("About");

        MenuItem ToolItem = new MenuItem("Tools");
        Menu ToolMenu = new Menu();
        CheckMenuItem ToolMenuShowCropRotation = new CheckMenuItem("Show crop rotation suggestion");

        //MenuItem WheatherDataItem = new MenuItem("Wheather Data");
        //Menu WheatherDataMenu = new Menu();
        //MenuItem 

        FileFilter f;

        public static bool ShowCropRotation;

        public MainWindowMenuBar()
        {
            GardenPlannerSettings settings = GardenPlannerSettings.GetSettings();

            FileItem.Submenu = FileMenu;
            FileMenu.Add(MenuItemNew);
            FileMenu.Add(MenuItemLoad);
            FileMenu.Add(MenuItemSave);
            FileMenu.Add(MenuItemImportFamilies);
            FileMenu.Add(MenuItemClose);
            Append(FileItem);

            MenuItemClose.Activated += CloseAction;
            MenuItemNew.Activated += NewAction;
            MenuItemSave.Activated += SaveAction;
            MenuItemLoad.Activated += LoadAction;
            MenuItemImportFamilies.Activated += ImportFamiliesAction;


            SettingsItem.Submenu = SettingsMenu;
            SettingsMenu.Add(LanguageItem);
            LanguageItem.Submenu = LanguageMenu;
            RadioMenuItem group = null;
            bool init = true;
            foreach (KeyValuePair<string, Translation> pair in Translation.BuiltIn)
            {

                RadioMenuItem radioItem = group == null ? new RadioMenuItem(pair.Key) : new RadioMenuItem(group, pair.Key);
                group = radioItem;
                radioItem.Active = settings.Language.Equals(pair.Key);
                radioItem.Activated += (sender, e) => {
                    if (init)//during program initialization this should not be executed
                        return;
                    settings.Language = pair.Key;
                    Translation.GetTranslation(pair.Key);
                    MainWindow.GetInstance().ResetForNewData();
                };
                LanguageMenu.Append(radioItem);
            }
            init = false;

            ShowAreaImagesItem.Active = settings.ShowAreaImages;
            ShowAreaImagesItem.Activated += (sender, e) =>
            {
                settings.ShowAreaImages = ShowAreaImagesItem.Active;
                GardenDrawingArea.ActiveInstance?.Draw();
            };
            SettingsMenu.Append(ShowAreaImagesItem);

            ShowPlantNames.Active = settings.ShowPlantNames;
            ShowPlantNames.Activated += (sender, e) =>
            {
                settings.ShowPlantNames = ShowPlantNames.Active;
                GardenDrawingArea.ActiveInstance?.Draw();
            };
            SettingsMenu.Append(ShowPlantNames);
            ShowVarietyNames.Active = settings.ShowVarietyNames;
            ShowVarietyNames.Activated += (sender, e) =>
            {
                settings.ShowVarietyNames = ShowVarietyNames.Active;
                GardenDrawingArea.ActiveInstance?.Draw();
            };
            SettingsMenu.Append(ShowVarietyNames);

            Append(SettingsItem);


            ToolItem.Submenu = ToolMenu;
            ToolMenu.Add(ToolMenuShowCropRotation);
            Append(ToolItem);

            ToolMenuShowCropRotation.Activated += ViewMenuShowCropRotation_Activated;


            HelpItem.Submenu = HelpMenu;
            HelpMenu.Add(MenuItemAbout);
            Append(HelpItem);

            MenuItemAbout.Activated += ShowAboutDialog;



            f = new FileFilter();
            f.AddPattern("*.gdata");
            f.Name = "Garden data";
        }

        class TestWindow : Window
        {
            public TestWindow() : base("test")
            {               
                if (Garden.GardenData.LoadedData != null)
                {
                    FamilyTreeView familyTreeView = new FamilyTreeView(Garden.GardenData.LoadedData);
                    Add(familyTreeView);
                }
            }

        }

        void ImportFamiliesAction(object sender, EventArgs e)
        {
            TestWindow testWindow = new TestWindow();
            testWindow.ShowAll();
        }

        void ViewMenuShowCropRotation_Activated(object sender, EventArgs e)
        {
            ShowCropRotation = ToolMenuShowCropRotation.Active;
            GardenDrawingArea.ActiveInstance?.Draw();
        }


        private void ShowAboutDialog(object sender, EventArgs args)
        {
            AboutDialog about = new AboutDialog();
            about.Authors = new string[]{ "Jahangmar" };
            about.ProgramName = "GardenPlanner";
            about.Version = MainClass.VERSION;
            about.Website = "https://github.com/jahangmar/GardenPlanner";
            about.WebsiteLabel = "GardenPlanner on github";
            about.TransientFor = MainWindow.GetInstance();
            about.Run();
            about.Destroy();
        }

        private void New()
        {
            Garden.GardenData.LoadedData = new Garden.GardenData("new project");
            Garden.GardenData.LoadedData = GardenPlanner.MainClass.TestData();
            MainWindow.GetInstance().Title = "Garden project '" + Garden.GardenData.LoadedData.Name + "'";
            MainWindow.GetInstance().ResetForNewData();
            Garden.GardenData.unsaved = false;
        }

        private void NewAction(object sender, EventArgs e)
        {
            if (Garden.GardenData.unsaved)
            {
                Dialog dialog = new MessageDialog(MainWindow.GetInstance(), DialogFlags.Modal, MessageType.Warning, ButtonsType.None, "There are unsaved changes. Continue to create new project?", new { });
                dialog.AddButton("Cancel", ResponseType.Cancel);
                dialog.AddButton("Discard", ResponseType.Reject);
                dialog.AddButton("Save and Continue", ResponseType.Apply);
                int response = dialog.Run();
                //System.Console.WriteLine("response " + result);
                if (response == (int)ResponseType.Cancel)
                {
                    //do nothing
                }
                else if (response == (int)ResponseType.Reject)
                {
                    New();
                }
                else if (response == (int)ResponseType.Apply)
                {
                    Save();
                    if (!Garden.GardenData.unsaved)
                    {

                    }

                }
                dialog.Destroy();
            }
            else
            {
                New();
            }
        }

        private void CloseAction(object sender, EventArgs e)
        {
            MainWindow.GetInstance().TryToClose();
        }



        private void LoadAction(object sender, EventArgs e)
        {
            FileChooserDialog fcd = new FileChooserDialog("Load garden data", MainWindow.GetInstance(), FileChooserAction.Open, "Cancel", ResponseType.Cancel, "Load", ResponseType.Apply);
            fcd.AddFilter(f);
            fcd.Response += (object o, ResponseArgs args) =>
            {
                switch (args.ResponseId)
                {
                    case ResponseType.Apply:
                        bool res = Garden.GardenData.Load(fcd.Filename);
                        if (!res)
                        {
                            MessageDialog md = new MessageDialog(MainWindow.GetInstance(), DialogFlags.Modal, MessageType.Warning, ButtonsType.Ok, "Failed to load '" + fcd.Filename + "': "+Garden.GardenData.ErrorMessage, new { });
                            md.Run();
                            md.Destroy();
                        }
                        else //successfully loaded
                        {
                            MainWindow.GetInstance().ResetForNewData();
                            Garden.GardenData.unsaved = false;
                        }

                        break;
                }
                fcd.Destroy();
            };
           fcd.Run();
            MainWindow.GetInstance().Title = "Garden project '" + Garden.GardenData.LoadedData.Name + "'";
        }

       public void Save() => SaveAction(null, null);

       private void SaveAction(object sender, EventArgs e)
        {
            FileChooserDialog fcd = new FileChooserDialog("Save garden data", MainWindow.GetInstance(), FileChooserAction.Save, "Cancel", ResponseType.Cancel, "Save", ResponseType.Apply);
            fcd.AddFilter(f);
            fcd.Response += (object o, ResponseArgs args) =>
            {
                switch (args.ResponseId)
                {
                    case ResponseType.Apply:
                        bool res = Garden.GardenData.Save(fcd.Filename);
                        if (!res)
                        {
                            MessageDialog md = new MessageDialog(MainWindow.GetInstance(), DialogFlags.Modal, MessageType.Warning, ButtonsType.Ok, "Failed to save '" + fcd.Filename + "': " + Garden.GardenData.ErrorMessage, new { });
                            md.Run();
                            md.Destroy();
                        }
                        else // successfull save
                        {
                            Garden.GardenData.unsaved = false;
                        }

                        break;
                }
                fcd.Destroy();
            };
            fcd.Run();
            MainWindow.GetInstance().Title = "Garden project '" + Garden.GardenData.LoadedData.Name + "'";
        }
    }
}
