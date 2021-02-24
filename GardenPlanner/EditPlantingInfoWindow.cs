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
    public class EditPlantingInfoWindow : EditWindow
    {
        SpinButton numberPlantsSpinButton;
        DateEntryBox plannedDateBox;
        CheckButton alreadyPlantedButton;
        DateEntryBox actualDateBox;

        public EditPlantingInfoWindow(PlantingInfo plantingInfo, System.Action<PlantingInfo> okAction) : base("Edit planting information")
        {
            GardenPlannerSettings settings = GardenPlannerSettings.GetSettings();

            AddControlButton("cancel", () =>
            {
                this.Destroy();
            });
            AddControlButton("ok", () =>
            {
                plantingInfo.Count = numberPlantsSpinButton.ValueAsInt;
                plantingInfo.PlannedPlantingDate = plannedDateBox.GetDate();
                plantingInfo.AlreadyPlanted = alreadyPlantedButton.Active;
                plantingInfo.PlantingDate = actualDateBox.GetDate();
                this.Destroy();
                okAction.Invoke(plantingInfo);
            });

            numberPlantsSpinButton = new SpinButton(1, settings.MaxPlantingCount, 1);
            numberPlantsSpinButton.Value = plantingInfo.Count;
            AddLabeledEntry("Number of plants", numberPlantsSpinButton);

            plannedDateBox = new DateEntryBox("planned planting date", DateEntryType.MiddleMonthDateEntry);
            plannedDateBox.SetDate(plantingInfo.PlannedPlantingDate);
            AddEntry(plannedDateBox);

            alreadyPlantedButton = new CheckButton("already planted")
            {
                Active = plantingInfo.AlreadyPlanted
            };
            alreadyPlantedButton.Clicked += (sender, e) =>
            {
                actualDateBox.Sensitive = alreadyPlantedButton.Active;
            };

            AddEntry(alreadyPlantedButton);

            actualDateBox = new DateEntryBox("planted on", DateEntryType.DayDateEntry)
            {
                Sensitive = alreadyPlantedButton.Active
            };
            actualDateBox.SetDate(plantingInfo.PlantingDate);
            AddEntry(actualDateBox);
        }

        /// <summary>
        /// Shows the planting info window for varities that already have been added to a planting.
        /// </summary>
        /// <param name="plantingInfo">Planting info.</param>
        /// <param name="action">Action.</param>
        /// <param name="planting">Planting.</param>
        /// <param name="varietyName">Variety name.</param>
        public static void ShowPlantingInfoWindow(PlantingInfo plantingInfo, System.Action<PlantingInfo> action, Planting planting, string varietyName)
        {
            EditPlantingInfoWindow window = new EditPlantingInfoWindow(plantingInfo, action);
            window.Title = $"Add {varietyName} to {planting.Name}";
            window.ShowAll();
        }

        /// <summary>
        /// Shows the planting info window for new varieties.
        /// </summary>
        /// <param name="action">Action.</param>
        /// <param name="planting">Planting.</param>
        /// <param name="varietyName">Variety name.</param>
        public static void ShowPlantingInfoWindow(System.Action<PlantingInfo> action, Planting planting, string varietyName) =>
            ShowPlantingInfoWindow(new PlantingInfo() { PlannedPlantingDate = planting.created, PlantingDate = planting.created }, action, planting, varietyName);
    }
}
