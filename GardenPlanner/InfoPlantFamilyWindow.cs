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

namespace GardenPlanner
{
    public class InfoPlantFamilyWindow : InfoAffectableWindow
    {
        private readonly PlantFamily Family;
        private readonly bool Create;

        private InfoPlantFamilyWindow(PlantFamily family, bool create, bool isEdited) : base("Info about " + family.Name, isEdited)
        {
            Family = family;
            Create = create;

            AddEntry(family.Name, infoView.tag_headline);

            AddEntry("(" + family.ScientificName + ")", infoView.tag_weak);

            AddEntry("Beschreibung: ", false);

            AddEntry(family.Description, infoView.tag_italic);

            ApplyTags();
        }

        public static void ShowWindow(PlantFamily family, bool isEdited = false, bool create = false)
        {
            InfoPlantFamilyWindow win = new InfoPlantFamilyWindow(family, create, isEdited);
            win.ShowAll();
        }

        protected override void Edit()
        {
            this.Destroy();
            EditPlantFamilyWindow.ShowWindow(Family, Create);
        }

    }
}
