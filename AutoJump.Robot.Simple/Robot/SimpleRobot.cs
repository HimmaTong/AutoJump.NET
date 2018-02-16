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
    /// <summary>
    /// 简单的机器人
    /// </summary>
    public class SimpleRobot : IGameRobot
    {
        /// <summary>
        /// 识别区域左边距
        /// </summary>
        const float PercentOffsetX = 1 / 30f;
        /// <summary>
        /// 识别区域上边距
        /// </summary>
        const float PercentOffsetY = 1 / 3f;
        /// <summary>
        /// 识别区域右边距（距离左边）
        /// </summary>
        const float PercentUponX = 29 / 30f;
        /// <summary>
        /// 识别区域下边距（距离顶部）
        /// </summary>
        const float PercentUponY = 2 / 3f;

        const float PercentCharacterOffset = 1.4f / 32f;
        const float PercentTargetBoxHeight = 0.13f;
        const float PercentCharacterHeight = 0.109f;
        const float PercentTargetBoxWidth = 0.42f;
        const float PercentCharacterWidth = 0.075f;
        [Obsolete("这个系数已被弃用,请使用PercentReferenceHeight代替", true)]
        const float PercentDistance = 2;
        const int ReferenceHeight = 2560;
        const float PercentReferenceHeight = 1;

        private Random _Random = new Random();

        public TapInformation GetNextTap(Bitmap image)
        {
            PositionPair pair = Solve(image);
            int duration = (int)(pair.Distance * ReferenceHeight / image.Height * PercentReferenceHeight);
            return new TapInformation(new Vector2(100 + _Random.Next(20), 100 + _Random.Next(20)), duration);
        }

        private PositionPair Solve(Bitmap image)
        {
            var width = image.Width;
            var height = image.Height;
            int offsetX = (int)(width * PercentOffsetX);
            int offsetY = (int)(height * PercentOffsetY);
            var uponX = (int)(width * PercentUponX);
            var uponY = (int)(height * PercentUponY);

            //识别目标落点和角色
            var clusters = GenerateClusters(image, offsetX, offsetY, uponX, uponY);
            Cluster cluster1 = clusters.Item1;
            Cluster cluster2 = clusters.Item2;

            Console.WriteLine($"cluster1:{cluster1.Vertices.Count}");
            Console.WriteLine($"cluster2:{cluster2.Vertices.Count}");

            //生成中心位置
            var center1 = cluster1.GetCenter();
            var center2 = cluster2.GetCenter() + new Vector2(0, height * PercentCharacterOffset);

            //可能发生了错误
            if (Math.Abs(center1.X - center2.X) < 10)
            {
                throw new Exception("识别错误");
            }

            using (var pg = Graphics.FromImage(image))
            {
                float radius = 4;

                //绘制聚类
                ClusterHelper.DrawCluster(image, Pens.Blue, cluster1);
                ClusterHelper.DrawCluster(image, Pens.Red, cluster2);

                //绘制连线
                pg.DrawLine(Pens.Red, center1.X, center1.Y, center2.X, center2.Y);
                //绘制矩形包围框
                pg.DrawRectangle(new Pen(Color.FromArgb(100, 0, 0, 255), 4), cluster1.GetRect());
                pg.DrawRectangle(new Pen(Color.FromArgb(100, 0, 255, 0), 4), cluster2.GetRect());

                //绘制中心点
                pg.FillEllipse(Brushes.Yellow, center1.X - radius, center1.Y - radius, radius * 2, radius * 2);
                pg.FillEllipse(Brushes.Yellow, center2.X - radius, center2.Y - radius, radius * 2, radius * 2);
                //绘制水平边框线
                pg.DrawLine(Pens.Black, 0, offsetY, width, offsetY);
                pg.DrawLine(Pens.Black, 0, uponY, width, uponY);
                //绘制垂直边框线
                pg.DrawLine(Pens.Black, offsetX, 0, offsetX, height);
                pg.DrawLine(Pens.Black, uponX, 0, uponX, height);
            }

            return new PositionPair(center1, center2);
        }

        private Tuple<Cluster, Cluster> GenerateClusters(Bitmap image, int offsetX, int offsetY, int uponX, int uponY)
        {
            var characterColor = Color.FromArgb(255, 54, 57, 100);
            var width = image.Width;
            var height = image.Height;
            int targetsCount = 4;
            List<Color> targetsOfBox = new List<Color>();
            Vertex TopOfTargetBox = null;
            Vertex TopOfCharacter = null;
            bool over1 = false;
            bool over2 = false;
            bool isAvoidHead = false;
            Vector2 avoidCenter = Vector2.Zero;


            for (int j = offsetY; j < uponY; j++)
            {
                for (int i = offsetX; i < uponX; i++)
                {
                    var current = image.GetPixel(i, j);
                    if (!over1)
                    {
                        //搜索落点顶端像素
                        if (isAvoidHead)
                        {
                            if ((avoidCenter - new Vector2(i, j)).Length() < width * 0.05)
                            {
                                continue;
                            }
                        }
                        if (ColorHelper.CompareBaseRGB(current, image.GetPixel(i, j - 1), 10) == false &&
                            ColorHelper.CompareBaseRGB(current, image.GetPixel(i - 1, j), 10) == false)
                        {
                            if (ColorHelper.CompareBaseRGB(characterColor, image.GetPixel(i, j + 0), 20) == false &&
                                ColorHelper.CompareBaseRGB(characterColor, image.GetPixel(i, j + 1), 30) == false &&
                                ColorHelper.CompareBaseRGB(characterColor, image.GetPixel(i, j + 3), 40) == false)
                            {
                                TopOfTargetBox = new Vertex(new Vector2(i, j + 3), image.GetPixel(i, j + 3));
                                for (int k = 0; k < targetsCount; k++)
                                {
                                    targetsOfBox.Add(image.GetPixel(i, j + 3 + 8 * k));
                                }
                                over1 = true;
                                if (isAvoidHead)
                                {
                                    over2 = true;
                                }
                            }
                            else
                            {
                                isAvoidHead = true;
                                avoidCenter = new Vector2(i, j) + new Vector2(0, width * 0.027f);
                                TopOfCharacter = new Vertex(new Vector2(i, j), current);
                            }
                        }
                    }
                    else
                    {
                        if (ColorHelper.CompareBaseRGB(current, characterColor, 25) == true)
                        {
                            TopOfCharacter = new Vertex(new Vector2(i, j), current);
                            over2 = true;
                        }
                    }
                }
                if (over2)
                {
                    break;
                }
            }

            if (TopOfTargetBox == null)
            {
                throw new Exception("未检测到目标落点");
            }

            if (TopOfCharacter == null)
            {
                throw new Exception("未检测到游戏角色");
            }

            var lowY1 = (int)TopOfTargetBox.Position.Y;
            var lowY2 = (int)TopOfCharacter.Position.Y;
            var uponY1 = (int)(TopOfTargetBox.Position.Y + height * PercentTargetBoxHeight);
            var uponY2 = (int)(TopOfCharacter.Position.Y + height * PercentCharacterHeight);
            uponY1 = uponY1 > uponY ? uponY : uponY1;
            uponY2 = uponY2 > uponY ? uponY : uponY2;
            if (uponY1 > uponY2) { uponY1 = uponY2; }

            var lowX1 = (int)(TopOfTargetBox.Position.X - width * PercentTargetBoxWidth / 2);
            var lowX2 = (int)(TopOfCharacter.Position.X - width * PercentCharacterWidth / 2);
            var uponX1 = (int)(TopOfTargetBox.Position.X + width * PercentTargetBoxWidth / 2);
            var uponX2 = (int)(TopOfCharacter.Position.X + width * PercentCharacterWidth / 2);
            uponX1 = uponX1 > uponX ? uponX : uponX1;
            uponX2 = uponX2 > uponX ? uponX : uponX2;

            //生成聚类
            var cluster1 = ClusterHelper.GetClusterByColors(image, targetsOfBox.ToArray(), lowX1, uponX1, lowY1, uponY1, 3);
            var cluster2 = ClusterHelper.GetClusterByColor(image, TopOfCharacter.Color, lowX2, uponX2, lowY2, uponY2, 35);

            //移除盒子聚类中属于角色底部附近的点
            var bottom = cluster2.GetCenter() + new Vector2(0, height * PercentCharacterOffset);
            var radius = height * PercentCharacterHeight * 0.6;
            cluster1.Vertices.RemoveAll(vertex => (vertex.Position - bottom).Length() < radius);
            return new Tuple<Cluster, Cluster>(cluster1, cluster2);
        }

    }
}
