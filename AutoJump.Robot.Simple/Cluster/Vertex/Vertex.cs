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
    /// 表示一个具有位置和颜色的顶点
    /// </summary>
    public class Vertex
    {
        public Vector2 Position { get; set; }
        public Color Color { get; set; }

        /// <summary>
        /// 创建并初始化一个实例
        /// </summary>
        public Vertex(Vector2 position, Color color)
        {
            Position = position;
            Color = color;
        }
    }
}
