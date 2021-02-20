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
using GardenPlanner.Garden;

namespace GardenPlanner
{
    class FamilyTreeView : ScrolledWindow
    {
        private TreeView treeView = new TreeView();

        public Action<GardenDataEntry> SelectedGardenDataEntry;
        public System.Action NewFamily = null;
        public System.Action<PlantFamily> NewPlant = null;
        public System.Action<Plant> NewVariety = null;

        private class NewFamilyEntry : GardenDataEntry
        {
            public NewFamilyEntry() : base("<new family>", "") { }
        }

        private class NewPlantEntry : GardenDataEntry
        {
            public PlantFamily Family;
            public NewPlantEntry(PlantFamily family) : base($"<new {family.Name} plant>", "")
            {
                this.Family = family;
            }
        }

        private class NewVarietyEntry : GardenDataEntry
        {
            public Plant Plant;
            public NewVarietyEntry(Plant plant) : base($"<new {plant.Name} variety>", "")
            {
                this.Plant = plant;
            }
        }

        private readonly CellRendererText cellRendererText = new CellRendererText();

        public FamilyTreeView(GardenData gardenData)
        {
            AddColumn("Family/Plant/Variety");

            TreeStore treeStore = new TreeStore(typeof(Garden.GardenDataEntry));
            foreach (Garden.PlantFamily family in gardenData.Families.Values)
            {
                TreeIter familyIter = treeStore.AppendValues(family);
                foreach (Garden.Plant plant in family.Plants.Values)
                {
                    TreeIter plantIter = treeStore.AppendValues(familyIter, plant);
                    foreach (Garden.PlantVariety variety in plant.Varieties.Values)
                    {
                        TreeIter varietyIter = treeStore.AppendValues(plantIter, variety);
                    }
                    treeStore.AppendValues(plantIter, new NewVarietyEntry(plant));
                }
                treeStore.AppendValues(familyIter, new NewPlantEntry(family));
            }
            treeStore.AppendValues(new NewFamilyEntry());

            treeView.Model = treeStore;

            treeView.Selection.Changed += (object sender, EventArgs e) =>
            {
                //System.Console.WriteLine("selected " + GetSelected().Name);
                GardenDataEntry selected = GetSelected();
                if (selected is NewFamilyEntry)
                    NewFamily?.Invoke();
                else if (selected is NewPlantEntry npe)
                    NewPlant?.Invoke(npe.Family);
                else if (selected is NewVarietyEntry nve)
                    NewVariety?.Invoke(nve.Plant);
                else
                    SelectedGardenDataEntry?.Invoke(GetSelected());
            };

            Add(treeView);
        }

        private Garden.GardenDataEntry GetSelected()
        {
            treeView.Selection.GetSelected(out TreeModel model, out TreeIter iter);
            return (Garden.GardenDataEntry)model.GetValue(iter, 0);
        }

        private void AddColumn(string label)
        {
            TreeViewColumn column = new TreeViewColumn()
            {
                Title = label,
            };
            column.PackStart(cellRendererText, true);
            column.SetCellDataFunc(cellRendererText, new TreeCellDataFunc(ShowDataEntry));
            treeView.AppendColumn(column);
        }

        private void ShowDataEntry(TreeViewColumn tree_column, CellRenderer cell, TreeModel tree_model, TreeIter iter)
        {
            Garden.GardenDataEntry data = (Garden.GardenDataEntry)tree_model.GetValue(iter, 0);
            CellRendererText textRenderer = (CellRendererText)cell;
            textRenderer.Text = data.Name;
            if (data is NewFamilyEntry || data is NewPlantEntry || data is NewVarietyEntry)
                textRenderer.Style = Pango.Style.Italic;
            else
                textRenderer.Style = Pango.Style.Normal;

            if (data is PlantFamily)
                textRenderer.Foreground = "blue";
            else if (data is Plant)
                textRenderer.Foreground = "green";
            else if (data is PlantVariety)
                textRenderer.Foreground = "red";
            else if (data is NewFamilyEntry || data is NewPlantEntry || data is NewVarietyEntry)
                textRenderer.Foreground = "black";
        }

    }
}
