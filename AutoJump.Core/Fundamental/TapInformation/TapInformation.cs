using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace AutoJump.Core
{
    /// <summary>
    /// 表示描述触压的对象
    /// </summary>
    public class TapInformation
    {
        /// <summary>
        /// 位置
        /// </summary>
        public Vector2 Position { get; set; }
        /// <summary>
        /// 持续毫秒数
        /// </summary>
        public int Duration { get; set; }

        public TapInformation(Vector2 position, int duration)
        {
            this.Position = position;
            this.Duration = duration;
        }
    }
}
