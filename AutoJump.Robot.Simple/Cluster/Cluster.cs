using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace AutoJump.Robot.Simple
{
    /// <summary>
    /// 聚类
    /// </summary>
    public class Cluster
    {
        /// <summary>
        /// 顶点集
        /// </summary>
        public List<Vertex> Vertices { get; set; } = new List<Vertex>();

        /// <summary>
        /// 返回当前聚类的中心位置
        /// </summary>
        public Vector2 GetCenter()
        {
            var x = Vertices.Average(v => v.Position.X);
            var y = Vertices.Average(v => v.Position.Y);
            return new Vector2(x, y);
        }

        /// <summary>
        /// 返回当前聚类的包围矩形
        /// </summary>
        public Rectangle GetRect()
        {
            var minX = (int)Vertices.Min(v => v.Position.X);
            var minY = (int)Vertices.Min(v => v.Position.Y);
            var maxX = (int)Vertices.Max(v => v.Position.X);
            var maxY = (int)Vertices.Max(v => v.Position.Y);
            return new Rectangle(minX, minY, maxX - minX, maxY - minY);
        }


    }
}
