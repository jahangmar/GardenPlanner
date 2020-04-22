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
        public int X, Y;

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

    }
}
