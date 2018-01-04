using AutoJump.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace AutoJump.Robot.Simple
{
    public static class ClusterHelper
    {
        /// <summary>
        /// 返回指定区域内与指定颜色相似的聚类
        /// </summary>
        public static Cluster GetClusterByColor(Bitmap image, Color target, int lowerX, int uponX, int lowerY, int uponY, int distance = 20)
        {
            Cluster result = new Cluster();
            for (int j = lowerY; j <= uponY; j++)
            {
                for (int i = lowerX; i <= uponX; i++)
                {
                    Color current = image.GetPixel(i, j);
                    if (ColorHelper.CompareBaseRGB(current, target, distance))
                    {
                        result.Vertices.Add(new Vertex(new Vector2(i, j), current));
                    }
                }
            }
            return result;
        }
        /// <summary>
        ///  返回指定区域内与指定颜色集合相似的聚类
        /// </summary>
        public static Cluster GetClusterByColors(Bitmap image, Color[] colors, int lowerX, int uponX, int lowerY, int uponY, int distance = 20)
        {
            var result = new Cluster();
            for (int j = lowerY; j < uponY; j++)
            {
                for (int i = lowerX; i < uponX; i++)
                {
                    var current = image.GetPixel(i, j);
                    for (int k = 0; k < colors.Length; k++)
                    {
                        if (ColorHelper.CompareBaseRGB(current, colors[k], distance))
                        {
                            result.Vertices.Add(new Vertex(new Vector2(i, j), current));
                        }
                    }
                }
            }
            return result;
        }
        /// <summary>
        /// 绘制聚类
        /// </summary>
        public static void DrawCluster(Bitmap image, Pen pen, Cluster cluster)
        {
            if ((cluster.Vertices.Count > 0))
            {
                using (var pg = Graphics.FromImage(image))
                {
                    for (int i = 0; i <= cluster.Vertices.Count - 1; i++)
                    {
                        var position = cluster.Vertices[i].Position;
                        pg.DrawRectangle(pen, position.X, position.Y, 1, 1);
                    }
                }
            }
        }
    }
}
