using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoJump.Core
{
    /// <summary>
    /// 像素数据
    /// </summary>
    public class PixelData
    {
        public Color[,] Colors;
        public int Width;
        public int Height;

        public PixelData(int width, int height)
        {
            Width = width;
            Height = height;
        }
        /// <summary>
        /// 从指定的颜色数组创建像素数据
        /// </summary>
        public static PixelData CreateFromColors(Color[,] colors)
        {
            int w = colors.GetUpperBound(0) + 1;
            int h = colors.GetUpperBound(1) + 1;
            return new PixelData(w, h) { Colors = colors };
        }
        /// <summary>
        /// 返回颜色数组的浅表副本
        /// </summary>
        public Color[,] GetColorsClone()
        {
            return (Color[,])Colors.Clone();
        }
    }
}
