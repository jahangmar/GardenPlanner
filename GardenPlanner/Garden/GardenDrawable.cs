﻿using Cairo;
namespace GardenPlanner.Garden
{
    public interface GardenDrawable
    {
        void Draw(Context context, int xoffset = 0, int yoffset = 0, double zoom = 1, int year = 0, int month = 0);
    }
}
