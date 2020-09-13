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

using Cairo;

namespace GardenPlanner.Garden
{
    /// <summary>
    /// A point in the garden has two coordinates in centimeters.
    /// </summary>
    public class GardenPoint
    {
        /// <summary>
        /// Amount of pixels that equal 1cm
        /// </summary>
        public const double STD_ZOOM = 1;

        /// <summary>
        /// Coordinate in cm
        /// </summary>
        public readonly int X, Y;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:GardenPlanner.Garden.GardenPoint"/> class.
        /// </summary>
        /// <param name="x">The x coordinate in cm.</param>
        /// <param name="y">The y coordinate in cm.</param>
        public GardenPoint(int x, int y)
        {
            X = x;
            Y = y;
        }

        //public Point ToCairoPoint(int zoom = STD_ZOOM) => new Point(X * zoom, Y * zoom);
        public PointD ToCairoPointD(int xoffset = 0, int yoffset = 0, double zoom = STD_ZOOM) => new PointD(xoffset + X * zoom, yoffset + Y * zoom);

        public bool Between(GardenPoint p1, GardenPoint p2)
        {
            GardenPoint vec = p1 - p2;
            //System.Console.WriteLine("p1 is " + p1);
            //System.Console.WriteLine("p2 is " + p2);
            //System.Console.WriteLine("this is " + this);
            //System.Console.WriteLine("vec is " + vec);
            //vec + p2 == p1
            //this == p2 + vec * f where 0 < f < 1
            for (float f = 0; f <= 1; f += 0.1f)
            {
                //System.Console.WriteLine("...checking " + (p2 + (vec * f)));
                if (this.Equals(p2 + (vec * f), 20))
                    return true;
            }
            return false;
        }

        public static GardenPoint operator *(GardenPoint p1, GardenPoint p2) =>
            new GardenPoint(p1.X * p2.X, p1.Y * p2.Y);

        public static GardenPoint operator *(GardenPoint p1, double d) =>
            new GardenPoint((int) (p1.X * d), (int) (p1.Y * d));

        public static GardenPoint operator +(GardenPoint p1, GardenPoint p2) =>
            new GardenPoint(p1.X + p2.X, p1.Y + p2.Y);

        public static GardenPoint operator +(GardenPoint p1, int i) =>
            new GardenPoint(p1.X + i, p1.Y + i);

        public static GardenPoint operator -(GardenPoint p1, GardenPoint p2) =>
            new GardenPoint(p1.X - p2.X, p1.Y - p2.Y);

        public static GardenPoint operator -(GardenPoint p1, int i) =>
            new GardenPoint(p1.X - i, p1.Y - i);

        public override bool Equals(object obj) =>
            Equals(obj, 1);

        public bool Equals(object obj, float epsilon)
        {
            if (obj is GardenPoint p)
                return (System.Math.Abs(p.X - this.X) < epsilon && System.Math.Abs(p.Y - this.Y) < epsilon);

            return false;
        }
    

        public override int GetHashCode() =>
            X.GetHashCode() ^ Y.GetHashCode();

        public override string ToString() =>
            "(" + X + ", " + Y + ")";
    }
}
