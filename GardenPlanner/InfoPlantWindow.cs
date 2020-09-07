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

            AddEntry("Aussaat draußen: " + plant.PlantOutsideDateRange);

            ApplyTags();

            ImageButton = new Button("Add Image");
            ActionButtonBox.Add(ImageButton);
            ImageButton.Clicked += ImageButton_Clicked;
        }

        void ImageButton_Clicked(object sender, EventArgs e)
        {
            FileChooserDialog fcd = new FileChooserDialog("Choose image for " + Plant.Name, this, FileChooserAction.Open, "Cancel", ResponseType.Cancel, "Load", ResponseType.Apply);
            FileFilter f = new FileFilter();
            f.AddPattern("*.png");
            f.Name = "png image";
            fcd.AddFilter(f);

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
