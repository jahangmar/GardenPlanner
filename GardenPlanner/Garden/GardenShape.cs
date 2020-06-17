using System;
using System.Collections.Generic;
using Cairo;

namespace GardenPlanner.Garden
{
    /// <summary>
    /// A shape of something in the garden represented as a list of <see cref="GardenPoint"/>
    /// </summary>
    public class GardenShape
    {
        private readonly List<GardenPoint> Points;

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

        //public bool ContainsPoint(GardenPoint p) => ContainsPoint(p.X, p.Y);

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
