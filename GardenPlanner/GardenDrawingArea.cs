using Gtk;
using Cairo;
using Gdk;

namespace GardenPlanner
{
    /// <summary>
    /// Area of the window where the garden is drawn and can be edited with the cursor
    /// </summary>
    public class GardenDrawingArea : Gtk.DrawingArea
    {
        private Garden.Garden Garden;

        public static double Zoom = 1;

        public GardenDrawingArea(Garden.Garden garden, SpinButton zoomButton, int width=800, int height=600)
        {
            this.Garden = garden;
            this.SetSizeRequest(width, height);
            this.AddEvents((int)(Gdk.EventMask.ButtonPressMask | Gdk.EventMask.ButtonReleaseMask | Gdk.EventMask.PointerMotionMask | Gdk.EventMask.KeyPressMask | Gdk.EventMask.KeyReleaseMask | Gdk.EventMask.AllEventsMask));

            this.ButtonPressEvent += delegate (object o, ButtonPressEventArgs args)
            {
                if (args.Event.Button == 1)
                {
                    System.Console.WriteLine("left mouse button clicked on " + Garden.Name + " at " + args.Event.X + "/" + args.Event.Y); //TODO

                }

            };

            zoomButton.ValueChanged += (object sender, System.EventArgs e) =>
            {
                Zoom = zoomButton.Value;
                if (IsDrawable)
                    Draw();
            };
        }

        protected override bool OnExposeEvent(EventExpose evnt)
        {
            Draw();
            return base.OnExposeEvent(evnt);
        }

        private void Draw()
        {
            Cairo.Context context = Gdk.CairoHelper.Create(this.GdkWindow);
            DrawBackground(context);
            DrawGrid(context);
            DrawGarden(context);
            context.Dispose();
        }

        private void DrawBackground(Context context)
        {
            context.MoveTo(0, 0);
            context.LineTo(this.Allocation.Width, 0);
            context.LineTo(this.Allocation.Width, this.Allocation.Height);
            context.LineTo(0, this.Allocation.Height);
            context.LineTo(0, 0);
            context.SetSourceColor(new Cairo.Color(1, 1, 1));
            context.FillPreserve();
            context.Stroke();
        }

        private void DrawGrid(Context context)
        {
            int width = this.Allocation.Width;
            int height = this.Allocation.Height;

            double factor = Zoom * 100;
            context.LineWidth = 0.5;
            for (int x=0; x*factor < width; x++)
            {
                if (x > 0)
                {
                    context.MoveTo(x * factor + 1, 10);
                    context.SetSourceColor(new Cairo.Color(0, 0, 0));
                    context.ShowText((x-1) + "m");
                }

                context.MoveTo(x * factor, 0);
                context.LineTo(x*factor, height);
            }

            context.SetSourceColor(new Cairo.Color(0, 0, 0, 0.5));
            context.Stroke();

            for (int y = 0; y*factor < height; y++)
            {
                if (y > 0)
                {
                    context.MoveTo(1, y * factor - 2);
                    context.SetSourceColor(new Cairo.Color(0, 0, 0));
                    context.ShowText((y-1) + "m");
                }

                context.MoveTo(0, y * factor);
                context.LineTo(width, y*factor);
            }
            context.SetSourceColor(new Cairo.Color(0, 0, 0, 0.5));
            context.Stroke();
        }

        private void DrawGarden(Context context)
        {

            /*
            context.MoveTo(new PointD(0, 0));
            context.LineTo(new PointD(100, 100));
            context.LineTo(new PointD(0, 100));
            context.LineTo(new PointD(300, 200));
            context.LineTo(new PointD(900, 200));
            context.LineTo(new PointD(500, 700));
            context.Stroke();
            */


            Garden.Draw(context, (int) (100*Zoom), (int) (100*Zoom), Zoom);
        }
    }
}
