using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoJump.Core
{
    public static class ColorHelper
    {
        public static bool CompareBaseRGB(Color color1, Color color2, float distance)
        {
            int r = (int)color1.R - (int)color2.R;
            int g = (int)color1.G - (int)color2.G;
            int b = (int)color1.B - (int)color2.B;
            int temp = (int)Math.Sqrt(r * r + g * g + b * b);
            return temp < distance;
        }
    }
}
