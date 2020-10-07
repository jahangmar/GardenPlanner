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
using GardenPlanner.Garden;

using Gtk;

namespace GardenPlanner
{
    public class PlantingCreationDialog : GardenAreaCreationDialog
    {
        ComboBoxEntry VarityBox = new ComboBoxEntry();
        Button VarietyRemoveButton = new Button("Remove");
        Dictionary<VarietyKeySeq, PlantingInfo> Varieties;
        VarietyKeySeq[] keys;

        protected PlantingCreationDialog(string title) : base(title)
        {

        }

        protected PlantingCreationDialog(string title, Planting planting) : base(title)
        {
            Varieties = new Dictionary<VarietyKeySeq, PlantingInfo>(planting.Varieties);
            SetUpVarieties();

            VarityBox.Changed += (object sender, EventArgs e) =>
            {
               VarietyRemoveButton.Sensitive = VarityBox.Active >= 0;
            };

            VarietyRemoveButton.Clicked += (object sender, EventArgs e) =>
            {
                Varieties.Remove(keys[VarityBox.Active]);
                SetUpVarieties();
            };

            ShowAll();
        }

        HBox VarietiesLabeledHBox;
        HBox VarietiesInnerHBox;

        private void SetUpVarieties()
        {
            string[] labels = new string[Varieties.Count];
            keys = new VarietyKeySeq[Varieties.Count];
            var en = Varieties.Keys.GetEnumerator();
            for (int i = 0; en.MoveNext() && i < Varieties.Count; i++)
            {
                keys[i] = en.Current;
                labels[i] = GardenData.LoadedData.GetVariety(en.Current).Name + " x " + Varieties[en.Current].Count;
            }
            VarityBox = new ComboBoxEntry(labels);
            VarietyRemoveButton.Sensitive = false;
            if (VarietiesLabeledHBox != null)
            {
                VarietiesInnerHBox.Remove(VarietyRemoveButton);
                EditVBox.Remove(VarietiesLabeledHBox);
            }
            VarietiesInnerHBox = new HBox();
            VarietiesInnerHBox.Add(VarityBox);
            VarietiesInnerHBox.Add(VarietyRemoveButton);
            VarietiesLabeledHBox = AddEditEntry("Varieties", VarietiesInnerHBox);
            ShowAll();
        }

        private static void SetValuesForCreation(Planting area, List<GardenPoint> points, PlantingCreationDialog dialog)
        {
            GardenAreaCreationDialog.SetValuesForCreation(area, points, dialog);
        }

        public static void ShowPlantingCreationDialog(List<GardenPoint> points, Action<Planting> action)
        {
            PlantingCreationDialog dialog = new PlantingCreationDialog("Create planting");

            GardenDrawingArea.ActiveInstance?.Draw();

            dialog.CreateButton.Clicked += (object sender, System.EventArgs e) =>
            {
                Planting area = new Planting(dialog.NameEntry.Text, dialog.DescrEntry.Text);
                SetValuesForCreation(area, points, dialog);
                action(area);
                GardenDrawingArea.ActiveInstance?.Draw();
                dialog.Destroy();
            };
        }

        private static void SetValuesForEdit(GardenArea area, PlantingCreationDialog dialog)
        {
            GardenAreaCreationDialog.SetValuesForEdit(area, dialog);
        }

        protected override void SetValues(GardenArea area)
        {
            Planting planting = area as Planting;
            planting.Varieties = this.Varieties;
            base.SetValues(area);
        }

        public static void ShowPlantingEditDialog(Planting area)
        {
            PlantingCreationDialog dialog = new PlantingCreationDialog("Edit planting '" + area.Name + "'", area);

            SetValuesForEdit(area, dialog);
        }
    }
}
