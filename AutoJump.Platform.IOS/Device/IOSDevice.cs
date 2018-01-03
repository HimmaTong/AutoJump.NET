using AutoJump.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace AutoJump.Platform.IOS
{
    /// <summary>
    /// 表示一个实现了<see cref="IDevice"/>的IOS设备
    /// </summary>
    public class IOSDevice : IDevice
    {
        public bool Availiable => throw new NotImplementedException();

        public Size Size => throw new NotImplementedException();

        public Image GetScreenImage()
        {
            throw new NotImplementedException();
        }

        public void Press(Vector2 position, int millionseconds)
        {
            throw new NotImplementedException();
        }
    }
}
