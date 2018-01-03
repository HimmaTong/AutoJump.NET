using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace AutoJump.Core
{
    /// <summary>
    /// 设备接口
    /// </summary>
    public interface IDevice
    {
        /// <summary>
        /// 设备是否生效
        /// </summary>
        bool Availiable { get; }
        /// <summary>
        /// 获取此设备的屏幕大小（以像素为单位）
        /// </summary>
        Size Size { get; }
        ///<summary>
        ///按压
        ///</summary>
        void Press(Vector2 position, int millionseconds);
        /// <summary>
        /// 返回设备屏幕图像
        /// </summary>
        Image GetScreenImage();
    }
}
