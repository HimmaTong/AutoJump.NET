using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace AutoJump.Robot.Simple
{
    /// <summary>
    /// 位置组合
    /// </summary>
    public class PositionPair
    {
        /// <summary>
        /// 起始点
        /// </summary>
        public Vector2 Start { get; set; }
        /// <summary>
        /// 目标点
        /// </summary>
        public Vector2 Target { get; set; }
        /// <summary>
        /// 返回两个位置之间的距离
        /// </summary>
        public float Distance { get => (Target - Start).Length(); }

        /// <summary>
        /// 创建并初始化一个实例
        /// </summary>
        public PositionPair(Vector2 start, Vector2 target)
        {
            this.Start = start;
            this.Target = target;
        }
    }
}
