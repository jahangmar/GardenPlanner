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
using GardenPlanner.Garden;
using Gtk;

namespace GardenPlanner
{
    public class InfoPlantWindow : InfoGrowableWindow
    {
        private readonly Plant Plant;
        private readonly bool Create;

        private Button ImageButton;

        private InfoPlantWindow(Plant plant, bool create, bool isEdited) : base("Info about " + plant.Name, isEdited)
        {
            Plant = plant;
            Create = create;

            AddEntry(plant.Name, infoView.headline);

            PlantFamily family = GardenData.LoadedData.GetFamily(plant.FamilyID);
            AddEntry("Gehört zu "+ family.Name , false);
            AddEntry("(" + plant.ScientificName + ")", infoView.weak);

            AddEntry("Beschreibung: ", false);

            AddEntry(plant.Description, infoView.italic);

            string feeder = "";
            switch (plant.FeederType)
            {
                case FeederType.Heavy:
                    feeder = "Starkzehrer";
                    break;
                case FeederType.Medium:
                    feeder = "Mittelzehrer";
                    break;
                case FeederType.Light:
                    feeder = "Schwachzeher";
                    break;
            }

            AddEntry("Nährstoffbedarf: " + feeder);

            AddEntry("Aussaat draußen: " + plant.PlantOutsideDateRange);

            ApplyTags();

            ImageButton = new Button("Add Image");
            ActionButtonBox.Add(ImageButton);
            ImageButton.Clicked += ImageButton_Clicked;
        }

        void ImageButton_Clicked(object sender, EventArgs e)
        {
            FileChooserDialog fcd = new FileChooserDialog("Choose image for " + Plant.Name, MainWindow.GetInstance(), FileChooserAction.Open, "Cancel", ResponseType.Cancel, "Load", ResponseType.Apply);
            FileFilter f = new FileFilter();
            f.AddPattern("*.png");
            f.Name = "png image";
            fcd.AddFilter(f);

            this.Sensitive = false;
            fcd.Response += (object o, ResponseArgs args) =>
            {
                switch (args.ResponseId)
                {
                    case ResponseType.Apply:
                        string filename = fcd.Filename;
                        try
                        {
                            System.IO.File.Copy(fcd.Filename, Plant.GetImageSurfacePath());
                        }
                        catch (Exception ex)
                        {
                            MessageDialog errord = new MessageDialog(MainWindow.GetInstance(), DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, "Exception: "+ex.Message, new { });
                            errord.Run();
                            errord.Destroy();
                            return;
                        }
                        MessageDialog md = new MessageDialog(MainWindow.GetInstance(), DialogFlags.Modal, MessageType.Info, ButtonsType.Ok, "Added image for "+Plant.Name, new { });
                        md.Run();
                        md.Destroy();
                        break;
                }
                fcd.Destroy();
            };
            fcd.Run();
            this.Sensitive = true;
        }

        public static void ShowWindow(Plant plant, bool isEdited = false, bool create = false)
        {
            InfoPlantWindow win = new InfoPlantWindow(plant, create, isEdited);
            win.ShowAll();
        }

        protected override void Edit()
        {
            this.Destroy();
            EditPlantWindow.ShowWindow(Plant, Create);
        }

    }
}
