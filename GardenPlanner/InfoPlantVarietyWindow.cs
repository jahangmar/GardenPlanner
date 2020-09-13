﻿// Copyright (c) 2020 Jahangmar
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

namespace GardenPlanner
{
    public class InfoPlantVarietyWindow : InfoGrowableWindow
    {
        private readonly PlantVariety Variety;
        private readonly bool Create;

        private InfoPlantVarietyWindow(PlantVariety variety, bool create, bool isEdited) : base("Info about "+variety.Name, isEdited)
        {
            Variety = variety;
            Create = create;

            AddEntry(variety.Name, infoView.headline);

            Plant plant = GardenData.LoadedData.GetPlant(variety.FamilyID, variety.PlantID);
            AddEntry(plant.Name + "-Sorte" +" ", false); 
            AddEntry("(" + plant.ScientificName + ")", infoView.weak);

            AddEntry("Beschreibung: ", false);

            AddEntry(variety.Description, infoView.italic);

            AddEntry("Aussaat draußen: " + variety.PlantOutsideDateRange);

            ApplyTags();
        }

        public static void ShowWindow(PlantVariety variety, bool isEdited = false, bool create=false)
        {
            InfoPlantVarietyWindow win = new InfoPlantVarietyWindow(variety, create, isEdited);
            win.ShowAll();
        }

        protected override void Edit()
        {
            this.Destroy();
            EditPlantVarietyWindow.ShowWindow(Variety, Create);
        }

    }
}
