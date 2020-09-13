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
using System.Collections.Generic;
using Cairo;
using Newtonsoft.Json;

namespace GardenPlanner.Garden
{
    /// <summary>
    /// Area in a garden with a shape and methods that can be applied to it
    /// </summary>
    public class GardenArea : GardenDataEntry, GardenDrawable
    {
        private const int LINE_WIDTH = 1;

        /// <summary>
        /// Time this area was created.
        /// </summary>
        public DateTime created;
        public DateTime removed;

        /// <summary>
        /// Identifier of the garden this area belongs to
        /// </summary>
        public string GardenID;

        public GardenShape Shape;

        /// <summary>
        /// Methods applied to a whole <see cref="Garden"/> (e.g. compost application or digging)
        /// or planting methods for <see cref="Planting"/> (e.g. directly sown, fertilized)
        /// or method applied to a specific <see cref="GardenArea"/>
        /// </summary>
        public List<BedMethod> Methods;

        public bool CheckDate(int year, int month) => (year == created.Year && month >= created.Month || year > created.Year) &&
            (year == removed.Year && month <= removed.Month || year < removed.Year) || year == 0 && month == 0;

        [JsonConstructor]
        public GardenArea(string name, string description) : base(name, description)
        {
            Methods = new List<BedMethod>();
            Shape = new GardenShape();
        }

        public GardenArea(string name, string description, int cyear, int cmonth, int ryear, int rmonth) : this(name, description)
        {
            SetCreated(cyear, cmonth);
            SetRemoved(ryear, rmonth);
        }

        public void SetCreated(int year, int month)
        {
            created = new DateTime(year, month, 1);
            if (removed < created)
                removed = created;
        }

        public void SetRemoved(int year, int month)
        {
            removed = new DateTime(year, month, 1);
            if (removed < created)
                created = removed;
        }

        public bool ContainsPointOnEdge(GardenPoint p, int xoffset = 0, int yoffset = 0, double zoom = GardenPoint.STD_ZOOM) =>
            Shape.ContainsPointOnEdge(p, xoffset, yoffset, zoom);

        public virtual void Draw(Context context, int xoffset = 0, int yoffset = 0, double zoom = 1, int year=0, int month=0)
        {
            if (CheckDate(year, month))
                this.Shape.Draw(context, xoffset, yoffset, new Color(0.3, 0.2, 0.2), new Color(0.4, 0.3, 0.3, 0.5), LINE_WIDTH, zoom);
        }
    }
}
