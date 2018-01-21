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
        /// <summary>
        /// 宽度
        /// </summary>
        public int Width { get; set; }
        /// <summary>
        /// 高度
        /// </summary>
        public int Height { get; set; }
        /// <summary>
        /// 颜色数组
        /// </summary>
        public Color[,] Colors { get; set; }

        /// <summary>
        /// 创建并初始化一个实例
        /// </summary>
        public PixelData(int width, int height)
        {
            this.Width = width;
            this.Height = height;
            this.Colors = new Color[width, height];
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
