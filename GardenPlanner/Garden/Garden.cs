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

using System.Collections.Generic;
using System;
using Cairo;
using Newtonsoft.Json;

namespace GardenPlanner.Garden
{
    /// <summary>
    /// A garden. This can be an area with multiple beds. Beds that are far away should be included in their own garden. A garden has plantings and methods.
    /// </summary>
    public class Garden : GardenArea, GardenDrawable
    {
        public Dictionary<string, Planting> Plantings = new Dictionary<string, Planting>();
        public Dictionary<string, GardenArea> MethodAreas = new Dictionary<string, GardenArea>();

        private const double LINE_WIDTH = 2;

        [JsonConstructor]
        public Garden(string name, string description) : base(name, description)
        {
        }

        public Garden(string name, string description, int cyear, int cmonth, int ryear, int rmonth) : base(name, description, cyear, cmonth, ryear, rmonth)
        {
        }

        public void AddPlanting(string key, Planting planting)
        {
            AddToDictionary(key, planting, Plantings);
            if (this.ID.Length == 0)
                throw new System.Exception($"{typeof(Garden).Name} must be added to {typeof(GardenData).Name} before adding {typeof(Planting).Name}");
            planting.GardenID = ID;
        }

        public void AddMethodArea(string key, GardenArea methodArea)
        {
            AddToDictionary(key, methodArea, MethodAreas);
            if (this.ID.Length == 0)
                throw new System.Exception($"{typeof(Garden).Name} must be added to {typeof(GardenData).Name} before adding {typeof(GardenArea).Name}");
            methodArea.GardenID = ID;
        }

        public void RemovePlanting(string key)
        {
            RemFromDictionary(key, Plantings);
        }

        public void RemovePlanting(Planting planting)
        {
            RemFromDictionary(planting, Plantings);
        }

        public void RemoveMethodArea(string key)
        {
            RemFromDictionary(key, MethodAreas);
        }

        public void RemoveMethodArea(GardenArea area)
        {
            RemFromDictionary(area, MethodAreas);
        }

        public override void Draw(Context context, int xoffset = 0, int yoffset = 0, double zoom=1, int year=0, int month=0)
        {
            if (!CheckDate(year, month))
            {
                this.Shape.Draw(context, xoffset, yoffset, new Color(0.2, 0.2, 0.2), new Color(0.8, 0.8, 0.8), LINE_WIDTH, zoom);
                context.MoveTo((new GardenPoint(10,50) + this.Shape.GetTopLeftPoint()).ToCairoPointD(xoffset, yoffset, zoom));
                context.ShowText("outdated");
            }
            else
            {
                this.Shape.Draw(context, xoffset, yoffset, new Color(0, 0, 0), new Color(1, 1, 1), LINE_WIDTH, zoom);
                foreach (KeyValuePair<string, Planting> pair in Plantings)
                {
                    pair.Value.Draw(context, xoffset, yoffset, zoom, year, month);
                }

                foreach (KeyValuePair<string, GardenArea> pair in MethodAreas)
                {
                    pair.Value.Draw(context, xoffset, yoffset, zoom, year, month);
                }
            }
        }
    }
}
