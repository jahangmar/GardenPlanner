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
        }
    }
}
