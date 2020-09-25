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

using Gtk;
using Cairo;
using Gdk;
using GardenPlanner.Garden;
using System.Collections.Generic;
using System;

namespace GardenPlanner
{
    /// <summary>
    /// Area of the window where the garden is drawn and can be edited with the cursor
    /// </summary>
    public class GardenDrawingArea : Gtk.DrawingArea
    {

        private const int GRID_SIZE = 100;

        public Garden.Garden Garden;

        public static double Zoom = 1;

        public static GardenDrawingArea ActiveInstance;

        public GardenPoint SelectedPoint;
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
                        GardenPoint gridPoint = SnapToGrid((int)(args.Event.X), (int)(args.Event.Y));
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
                        MenuItem item5 = new MenuItem("Edit point");
                        item5.Sensitive = SelectedPoint != null;
                        item5.Activated += (sender, e) => PointInputWindow.ShowWindow("Set new point", Math.Min(SelectedPoint.X, SelectedPoint.Y)-100, Math.Max(SelectedPoint.X, SelectedPoint.Y) + 100, SelectedPoint.X, SelectedPoint.Y,
                            (int x, int y) => { SelectedArea.Shape.ModPoint(SelectedPoint, new GardenPoint(x, y), true); SelectedPoint = null; Draw(); });

                        Menu menu = new Menu() {
                            item1, item2, item3, item4, item5
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
            double grid = (GRID_SIZE/4)*Zoom;
            GardenPoint result = new GardenPoint(-GRID_SIZE + (int)System.Math.Round((int)(System.Math.Round((double)(x / grid)) * grid) / Zoom), -GRID_SIZE + (int)System.Math.Round((int)(System.Math.Round((double)(y / grid)) * grid) / Zoom));
            return result;
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
                        if (SelectedArea != null)
                            SelectedPoint = SelectedArea.GetPointInRange(clicked, 10, XOffset(), YOffset(), Zoom);
                        else
                            SelectedPoint = area.GetPointInRange(clicked, 10, XOffset(), YOffset(), Zoom);
                        SelectedArea = area;
                        return true;
                    }

                foreach (GardenArea area in Garden.Plantings.Values)
                    if (area.CheckDate(year, month) && area.ContainsPointOnEdge(clicked, XOffset(), YOffset(), Zoom))
                    {
                        if (SelectedArea != null)
                            SelectedPoint = SelectedArea.GetPointInRange(clicked, 10, XOffset(), YOffset(), Zoom);
                        else
                            SelectedPoint = area.GetPointInRange(clicked, 10, XOffset(), YOffset(), Zoom);
                        SelectedArea = area;
                        return true;
                    }
            }

            if (Garden.ContainsPointOnEdge(clicked, XOffset(), YOffset(), Zoom))
            {
                SelectedArea = Garden;
                SelectedPoint = Garden.GetPointInRange(clicked, 10, XOffset(), YOffset(), Zoom);
                return true;
            }

            return false;
        }

        protected override bool OnExposeEvent(EventExpose evnt)
        {
            Draw();
            return base.OnExposeEvent(evnt);
        }

        private int XOffset() => (int)(GRID_SIZE * Zoom);
        private int YOffset() => (int)(GRID_SIZE * Zoom);

        public void Draw()
        {
            if (ActiveInstance == null)
                return;
            Cairo.Context context = Gdk.CairoHelper.Create(this.GdkWindow);
            DrawBackground(context);
            DrawGrid(context);
            DrawGarden(context);
            DrawSelection(context);
            DrawCropRotation(context);

            context.Dispose();
        }

        private void DrawCropRotation(Context context)
        {

            if (MainWindowMenuBar.ShowCropRotation)
            {
                foreach (Planting planting in Garden.Plantings.Values)
                {

                    bool incomp = false;

                    if (MainWindow.GetInstance().SelectedEntry is Affectable aff)
                    {
                        foreach (VarietyKeySeq varietyKeySeq in planting.Varieties.Keys)
                        {
                            if (aff.CheckIncompatiblePlants(varietyKeySeq.PlantKey) || aff.CheckIncompatibleFamilies(varietyKeySeq.FamilyKey))
                            {
                                incomp = true;
                                break;
                            }
                        }
                    }
                    else
                    {
                        break;
                    }

                    if (!incomp)
                    {
                        continue;
                    }

                    void MarkLevel(int i)
                    {
                        Cairo.Color color;
                        switch (i)
                        {
                            case 0:
                                color = new Cairo.Color(1, 0, 0, 0.5);
                                break;
                            case 1:
                                color = new Cairo.Color(1, 0.5, 0, 0.5);
                                break;
                            case 2:
                                color = new Cairo.Color(1, 1, 0, 0.5);
                                break;
                            default:
                                color = new Cairo.Color(1, 1, 0.3, 0.5);
                                break;
                        }
                        planting.Shape.Draw(context, XOffset(), YOffset(), color, color, 1, Zoom);
                    }

                    int year = MainWindow.GetInstance().GetYear();
                    int month = MainWindow.GetInstance().GetMonth();
                    DateTime current = new DateTime(year, month, 1);

                    if (planting.CheckDate(year, month)) //exists during current time
                    {
                        MarkLevel(0);
                    }
                    else if (planting.removed < current) //exists in the past
                    {
                        DateRange range = new DateRange(planting.removed.Year, planting.removed.Month, current.Year, current.Month);
                        if (range.GetRangeInYears() < 3)
                            MarkLevel((int) Math.Floor(range.GetRangeInYears()));
                    }
                }
            }
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

            if (SelectedPoint != null)
            {
                Cairo.PointD pointd = SelectedPoint.ToCairoPointD(XOffset(), YOffset(), Zoom);
                context.MoveTo(pointd.X - 4, pointd.Y - 4);
                context.LineTo(pointd.X + 4, pointd.Y - 4);
                context.LineTo(pointd.X + 4, pointd.Y + 4);
                context.LineTo(pointd.X - 4, pointd.Y + 4);
                context.LineTo(pointd.X - 4, pointd.Y - 4);
                context.SetSourceRGB(0.3, 0.3, 0.3);
                context.Fill();
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

            double factor = Zoom * GRID_SIZE;
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
