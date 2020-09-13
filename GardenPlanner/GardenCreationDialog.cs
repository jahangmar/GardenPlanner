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

namespace GardenPlanner
{
    public class GardenCreationDialog : GardenAreaCreationDialog
    {
        protected GardenCreationDialog() : base("Create new garden")
        {

        }

        public static void ShowGardenCreationDialog(List<GardenPoint> points, Action<Garden.Garden> action)
        {
            GardenCreationDialog dialog = new GardenCreationDialog();

            dialog.CreateButton.Clicked += (object sender, System.EventArgs e) =>
            {
                Garden.Garden area = new Garden.Garden(dialog.NameEntry.Text, dialog.DescrEntry.Text);
                SetValuesForCreation(area, points, dialog);
                action(area);
                GardenDrawingArea.ActiveInstance?.Draw();
                dialog.Destroy();
            };
        }
    }
}
