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
using Cairo;
using Newtonsoft.Json;

namespace GardenPlanner.Garden
{
    /// <summary>
    /// A shape of something in the garden represented as a list of <see cref="GardenPoint"/>
    /// </summary>
    public class GardenShape
    {
        [JsonProperty]
        private List<GardenPoint> Points;

        public GardenShape()
        {
            Points = new List<GardenPoint>();
        }

        public void AddPoint(GardenPoint gardenPoint, int idx)
        {
            if (idx < 0)
                idx = 0;
            if (idx > Points.Count)
                idx = Points.Count;

            Points.Insert(idx, gardenPoint);
        }

        public void AddPoint(GardenPoint gardenPoint) => AddPoint(gardenPoint, Points.Count);

        public void AddPoint(int x, int y, int idx) => AddPoint(new GardenPoint(x, y), idx);
        public void AddPoint(int x, int y) => AddPoint(x, y, Points.Count);

        public void AddPoints(List<GardenPoint> points)
        {
            Points.AddRange(points);
        }

        public bool FinishPoints()
        {
            if (Points.Count == 0)
                return false;

            AddPoint(Points[0]);
            return true;
        }

        public void ModPoint(GardenPoint gardenPoint, int idx)
        {
            if (idx < 0 || idx > Points.Count)
                throw new IndexOutOfRangeException($"index {idx} is out of range [0, ..., {Points.Count-1}]");

            Points[idx] = gardenPoint;
        }

        public void ModPoint(GardenPoint oldPoint, GardenPoint newPoint, bool multiple = false)
        {
            List<int> mod = new List<int>();
            for (int i = 0; i < Points.Count; i++)
            {
                GardenPoint point = Points[i];
                if (point == oldPoint || point.X == oldPoint.X && point.Y == oldPoint.Y)
                {
                    mod.Add(i);
                    if (!multiple)
                        break;
                }
            }

            foreach (int idx in mod)
            {
                ModPoint(newPoint, idx);
            }
        }

        public void ClearPoints() => Points.Clear();

        public void RemovePoint(int x, int y)
        {
            Points.RemoveAll((GardenPoint p) => p.X == x && p.Y == y);
        }

        public void RemovePoint(GardenPoint point) => RemovePoint(point.X, point.Y);

        public List<GardenPoint> GetPoints() => Points;

        public bool ContainsPointOnEdge(GardenPoint p, int xoffset=0, int yoffset=0, double zoom = GardenPoint.STD_ZOOM)
        {
            if (Points.Count < 2)
                return false;

            GardenPoint offset = new GardenPoint(xoffset, yoffset);

            bool found = p.Between(Points[Points.Count - 1]*zoom+offset, Points[0]*zoom+offset);

            for (int i=0; !found && i < Points.Count-1; i++)
            {
                found = p.Between(Points[i]*zoom+offset, Points[i + 1]*zoom+offset);
            }
            return found;
        }

        /// <summary>
        /// Checks for point near the given point <paramref name="cp"/> within a distance of <paramref name="range"/>
        /// </summary>
        public GardenPoint GetPointInRange(GardenPoint cp, int range = 10, int xoffset = 0, int yoffset = 0, double zoom = GardenPoint.STD_ZOOM)
        {
            GardenPoint result = null;
            GardenPoint offset = new GardenPoint(xoffset, yoffset);
            foreach (GardenPoint sp in Points)
            {
                GardenPoint zoomed = sp * zoom + offset;
                if (Math.Abs(zoomed.X - cp.X) < range && Math.Abs(zoomed.Y - cp.Y) < range)
                {
                    result = sp;
                    break;
                }
            }
            return result;
        }

        /// <summary>
        /// Returns the point that is the most left of all points and the most top point of those
        /// </summary>
        public GardenPoint GetTopLeftPoint()
        {
            if (Points.Count == 0)
                return new GardenPoint(0, 0);

            GardenPoint result = Points[0];
            foreach (GardenPoint p in Points)
            {
                if (p.X < result.X)
                {
                    result = p;
                    continue;
                }
                else if (p.X == result.X && p.Y < result.Y)
                {
                    result = p;
                }
            }
            return result;

        }

        public void Draw(Context context, int xoffset, int yoffset, Color lineColor, Color fillColor, double lineWidth, double zoom = GardenPoint.STD_ZOOM)
        {
            context.LineCap = LineCap.Round;
            if (Points.Count > 0)
            {
                context.MoveTo(Points[0].ToCairoPointD(xoffset, yoffset, zoom));
                context.SetSourceColor(fillColor);
                context.LineWidth = lineWidth;
                foreach (GardenPoint point in Points.GetRange(1, Points.Count - 1))
                {
                    context.LineTo(point.ToCairoPointD(xoffset, yoffset, zoom));
                }
                if (fillColor.A < 1)
                    context.FillPreserve();
                context.SetSourceColor(lineColor);
                context.Stroke();
            }
        }
    }
}
