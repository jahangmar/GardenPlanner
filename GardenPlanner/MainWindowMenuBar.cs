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
        MenuItem MenuItemClose = new MenuItem("Close");

        MenuItem HelpItem = new MenuItem("Help");
        Menu HelpMenu = new Menu();
        //MenuItem MenuItemGetHelp = new MenuItem("")
        MenuItem MenuItemAbout = new MenuItem("About");

        //MenuItem WheatherDataItem = new MenuItem("Wheather Data");
        //Menu WheatherDataMenu = new Menu();
        //MenuItem 


        FileFilter f;

        public MainWindowMenuBar()
        {
            FileItem.Submenu = FileMenu;
            FileMenu.Add(MenuItemNew);
            FileMenu.Add(MenuItemLoad);
            FileMenu.Add(MenuItemSave);
            FileMenu.Add(MenuItemClose);
            Append(FileItem);

            MenuItemClose.Activated += CloseAction;
            MenuItemNew.Activated += NewAction;
            MenuItemSave.Activated += SaveAction;
            MenuItemLoad.Activated += LoadAction;


            HelpItem.Submenu = HelpMenu;
            HelpMenu.Add(MenuItemAbout);
            Append(HelpItem);

            MenuItemAbout.Activated += ShowAboutDialog;

            f = new FileFilter();
            f.AddPattern("*.gdata");
            f.Name = "Garden data";
        }

        private void ShowAboutDialog(object sender, EventArgs args)
        {
            AboutDialog about = new AboutDialog();
            about.Authors = new string[]{ "Jahangmar" };
            about.ProgramName = "GardenPlanner";
            about.Version = MainClass.VERSION;
            about.Website = "https://github.com/jahangmar/";
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
        }

        private void NewAction(object sender, EventArgs e)
        {
            if (Garden.GardenData.LoadedData.unsaved)
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
                    if (!Garden.GardenData.LoadedData.unsaved)
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
                        else
                        {
                            MainWindow.GetInstance().ResetForNewData();
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

                        break;
                }
                fcd.Destroy();
            };
            fcd.Run();
            MainWindow.GetInstance().Title = "Garden project '" + Garden.GardenData.LoadedData.Name + "'";
        }
    }
}
