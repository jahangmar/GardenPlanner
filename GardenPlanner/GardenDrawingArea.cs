using Gtk;
using Cairo;
using Gdk;
using GardenPlanner.Garden;
using System.Collections.Generic;

namespace GardenPlanner
{
    /// <summary>
    /// Area of the window where the garden is drawn and can be edited with the cursor
    /// </summary>
    public class GardenDrawingArea : Gtk.DrawingArea
    {
        public Garden.Garden Garden;

        public static double Zoom = 1;

        public static GardenDrawingArea ActiveInstance;

        public GardenArea SelectedArea;
        public List<GardenPoint> NewPoints = new List<GardenPoint>();

        public GardenDrawingArea(Garden.Garden garden, SpinButton zoomButton, int width=800, int height=600)
        {
            this.Garden = garden;
            this.SetSizeRequest(width, height);
            this.AddEvents((int)(Gdk.EventMask.ButtonPressMask | Gdk.EventMask.ButtonReleaseMask | Gdk.EventMask.PointerMotionMask
            | Gdk.EventMask.KeyPressMask | Gdk.EventMask.KeyReleaseMask | Gdk.EventMask.AllEventsMask));


            this.ButtonPressEvent += delegate (object o, ButtonPressEventArgs args)
            {
                if (args.Event.Button == 1)
                {
                    if (MainWindow.GetInstance().AreaNewButton.Active)
                    {
                        UndoSelection();
                        GardenPoint gridPoint = SnapToGrid((int)args.Event.X, (int)args.Event.Y);
                        NewPoints.Add(gridPoint);
                        Draw();
                    }
                    else if (CheckAreaClick((int)args.Event.X, (int)args.Event.Y))
                    {
                        MakeSelection();
                    }
                    else
                    {
                        UndoSelection();
                    }
                }
                else if (args.Event.Button == 3 || args.Event.Button == 2)
                {
                    if (CheckAreaClick((int)args.Event.X, (int)args.Event.Y) && SelectedArea != null)
                    {
                        MakeSelection();

                        MenuItem item1 = new MenuItem("Set created date");
                        item1.Activated += (sender, e) => DateInputWindow.ShowWindow("Set created date", (int y,int m) => { SelectedArea.SetCreated(y, m); });
                        MenuItem item2 = new MenuItem("Set removed date");
                        item2.Activated += (sender, e) => DateInputWindow.ShowWindow("Set removed date", (int y, int m) => { SelectedArea.SetRemoved(y, m); });
                        MenuItem item3 = new MenuItem("Remove variety...");
                        item3.Sensitive = false;
                        MenuItem item4 = new MenuItem("Remove area");
                        item4.Activated += (sender, e) => MainWindow.GetInstance().AreaDeleteButton.Activate();
                        Menu menu = new Menu() {
                            item1, item2, item3, item4
                        };

                        menu.AttachToWidget(this, null);
                        menu.ShowAll();
                        menu.Popup();
                    }
                }
            };                    

            zoomButton.ValueChanged += (object sender, System.EventArgs e) =>
            {
                Zoom = zoomButton.Value;
                if (IsDrawable)
                    Draw();
            };

            this.QueryTooltip += (object o, QueryTooltipArgs args) =>
            {
                System.Console.WriteLine("tooltip drawing area");
            };
            zoomButton.QueryTooltip += (object o, QueryTooltipArgs args) =>
            {
                System.Console.WriteLine("tooltip zoom button");
            };

        }

        public void MakeSelection()
        {
            MainWindow win = MainWindow.GetInstance();
            win.SelectGardenEntry(SelectedArea);
            win.PlantAddButton.Sensitive = SelectedArea is Planting && win.SelectedEntry is PlantVariety;
            win.AreaEditButton.Sensitive = true;
            Draw();

            if (SelectedArea is Garden.Garden g)
            {
                TooltipText = "Garden '" + g.Name + "'";
            }
            else if (SelectedArea is Planting p)
            {
                string s = "";
                foreach (var k in p.Varieties.Keys)
                {
                    PlantVariety v = GardenData.LoadedData.GetVariety(k);
                    s += v.Name + ", ";
                }
                if (s.Length > 0)
                {
                    s = s.Substring(0, s.Length - 2);
                    TooltipText = "Planting '" + p.Name + "': " + s;
                }
                else
                {
                    TooltipText = "Planting '" + p.Name + "'";
                }

            }
            else if (SelectedArea is GardenArea a)
            {
                TooltipText = "MethodArea '" + a.Name + "'";
            }
        }

        public void UndoSelection()
        {
            MainWindow win = MainWindow.GetInstance();
            SelectedArea = null;
            win.PlantAddButton.Sensitive = false;
            win.AreaEditButton.Sensitive = false;
            Draw();
            TooltipText = "";
            win.ShowEmptyAreaSelectionInfo();
        }

        private GardenPoint SnapToGrid(int x, int y)
        {
            double grid = (25);
            return new GardenPoint(-XOffset() + (int)(System.Math.Round((double) (x / grid)) * grid), -YOffset() + (int)(System.Math.Round((double) (y / grid)) * grid));
        }

        /// <summary>
        /// Checks if an area of the garden was selected by clicking the edge of that area.
        /// </summary>
        /// <returns><c>true</c>, if click selection was made, <c>false</c> otherwise.</returns>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        private bool CheckAreaClick(int x, int y)
        {
            GardenPoint clicked = new GardenPoint(x, y);
            int year = MainWindow.GetInstance().GetYear();
            int month = MainWindow.GetInstance().GetMonth();

            if (Garden.CheckDate(year, month))
            {
                foreach (GardenArea area in Garden.MethodAreas.Values)
                    if (area.CheckDate(year, month) && area.ContainsPointOnEdge(clicked, XOffset(), YOffset(), Zoom))
                    {
                        SelectedArea = area;
                        return true;
                    }

                foreach (GardenArea area in Garden.Plantings.Values)
                    if (area.CheckDate(year, month) && area.ContainsPointOnEdge(clicked, XOffset(), YOffset(), Zoom))
                    {
                        SelectedArea = area;
                        return true;
                    }
            }

            if (Garden.ContainsPointOnEdge(clicked, XOffset(), YOffset(), Zoom))
            {
                SelectedArea = Garden;
                return true;
            }

            return false;
        }

        protected override bool OnExposeEvent(EventExpose evnt)
        {
            Draw();
            return base.OnExposeEvent(evnt);
        }

        private int XOffset() => (int)(100 * Zoom);
        private int YOffset() => (int)(100 * Zoom);

        public void Draw()
        {
            if (ActiveInstance == null)
                return;
            Cairo.Context context = Gdk.CairoHelper.Create(this.GdkWindow);
            DrawBackground(context);
            DrawGrid(context);
            DrawGarden(context);
            DrawSelection(context);
            context.Dispose();
        }

        private void DrawSelection(Context context)
        {
            if (SelectedArea != null)
                SelectedArea.Shape.Draw(context, XOffset(), YOffset(), new Cairo.Color(0, 0, 0), new Cairo.Color(0.2, 0.2, 0.2, 0.2), 3, Zoom);

            if (NewPoints.Count > 0)
            {
                context.MoveTo(NewPoints[0].ToCairoPointD(XOffset(), YOffset(), Zoom));
                NewPoints.ForEach((GardenPoint p) => context.LineTo(p.ToCairoPointD(XOffset(), YOffset(), Zoom)));
                context.Stroke();
            }
        }

        private void DrawBackground(Context context)
        {
            context.MoveTo(0, 0);
            context.LineTo(this.Allocation.Width, 0);
            context.LineTo(this.Allocation.Width, this.Allocation.Height);
            context.LineTo(0, this.Allocation.Height);
            context.LineTo(0, 0);
            context.SetSourceColor(new Cairo.Color(0.95, 0.95, 0.95));
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
            Garden.Draw(context, XOffset(), YOffset(), Zoom, MainWindow.GetInstance().GetYear(), MainWindow.GetInstance().GetMonth());
        }
    }
}
