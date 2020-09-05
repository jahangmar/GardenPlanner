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
        }

        private void NewAction(object sender, EventArgs e)
        {

        }

        private void CloseAction(object sender, EventArgs e)
        {
            MainWindow.GetInstance().TryToClose();
        }

        private void LoadAction(object sender, EventArgs e)
        {
            FileChooserDialog fcd = new FileChooserDialog("Load garden data", MainWindow.GetInstance(), FileChooserAction.Open, "Cancel", ResponseType.Cancel, "Load", ResponseType.Apply);
            FileFilter f = new FileFilter();
            f.AddPattern("*.gdata");
            f.Name = "Garden data";
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

                        break;
                }
                fcd.Destroy();
            };
           fcd.Run();
        }

        private void SaveAction(object sender, EventArgs e)
        {

        }
    }
}
