using System;
using System.Drawing;
using System.Numerics;
using AutoJump.Core;
using System.Diagnostics;

namespace AutoJump.Platform.Android
{
    /// <summary>
    /// 表示一个实现了<see cref="IDevice"/>的Android设备
    /// </summary>
    public class AndroidDevice : IDevice
    {
        public bool Availiable => CheckConnection();
        public Size Size => _Size;

        /// <summary>
        /// Adb.exe的绝对路径
        /// </summary>
        private string _AdbPath { get; set; }
        /// <summary>
        /// 屏幕分辨率
        /// </summary>
        private Size _Size;

        /// <summary>
        /// 创建并初始化一个实例
        /// </summary>
        public AndroidDevice(string adbPath)
        {
            this._AdbPath = adbPath;
        }

        public Image GetScreenImage()
        {
            Bitmap result = null;
            var tempFileName = "temp.png";
            Command("shell screencap -p /sdcard/" + tempFileName);
            Command("pull /sdcard/" + tempFileName);
            if (System.IO.File.Exists(tempFileName))
            {
                using (var img = Image.FromFile(tempFileName))
                {
                    result = new Bitmap(img);
                }

                GC.Collect();
                if (System.IO.File.Exists(tempFileName))
                {
                    try
                    {
                        System.IO.File.Delete(Environment.CurrentDirectory + tempFileName);
                    }
                    catch
                    {
                    }
                }
            }
            _Size = result.Size;
            return result;
        }

        public void Press(Vector2 position, int millionseconds)
        {
            var target = position + new Vector2(100, 100);
            Command($"shell input swipe {position.X} {position.Y} {target.X} {target.Y} {millionseconds}");
        }

        /// <summary>
        /// 返回一个<see cref="Boolean"/>值指示该设备是否连接
        /// </summary>
        private bool CheckConnection()
        {
            var text = Command("shell getprop ro.product.model");
            if (text.Contains("no devices") || string.IsNullOrWhiteSpace(text))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        /// <summary>
        /// 向Adb发送指定的参数命令
        /// </summary>
        private string Command(string arguments)
        {
            string result = string.Empty;
            using (Process p = new Process())
            {
                p.StartInfo.FileName = this._AdbPath;
                p.StartInfo.Arguments = arguments;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardInput = true;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardError = true;
                p.StartInfo.CreateNoWindow = true;
                p.Start();
                result = p.StandardOutput.ReadToEnd();
                p.Close();
            }
            return result;
        }
    }
}
