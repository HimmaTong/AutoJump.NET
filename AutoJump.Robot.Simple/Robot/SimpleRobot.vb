Imports System.Drawing
Imports System.Numerics
Imports AutoJump.Core
''' <summary>
''' 简单的机器人
''' </summary>
Public Class SimpleRobot
    Implements IGameRobot
    ''' <summary>
    ''' 识别区域横向偏移百分比
    ''' </summary>
    Const PercentOffsetX As Single = 1 / 30
    ''' <summary>
    ''' 识别区域纵向偏移百分比
    ''' </summary>
    Const PercentOffsetY As Single = 1 / 3
    ''' <summary>
    ''' 识别区域横向上限百分比
    ''' </summary>
    Const PercentUponX As Single = 29 / 30
    ''' <summary>
    ''' 识别区域纵向上限百分比
    ''' </summary>
    Const PercentUponY As Single = 2 / 3
    ''' <summary>
    ''' 角色中心偏移百分比（由中心偏移到底部）
    ''' </summary>
    Const PercentCharacterOffset As Single = 1.4 / 32

    ''' <summary>
    ''' 盒子扫描高度百分比
    ''' </summary>
    Const PercentTargetBoxHeight As Single = 0.13
    ''' <summary>
    ''' 角色扫描高度百分比
    ''' </summary>
    Const PercentCharacterHeight As Single = 0.109

    ''' <summary>
    ''' 盒子扫描宽度百分比
    ''' </summary>
    Const PercentTargetBoxWidth As Single = 0.42
    ''' <summary>
    ''' 角色扫描宽度百分比
    ''' </summary>
    Const PercentCharacterWidth As Single = 0.075

    ''' <summary>
    ''' 跳跃系数
    ''' </summary>
    <Obsolete("这个比例系数已被弃用,改为自动计算", True)>
    Const PercentDistance As Single = 2

    ''' <summary>
    ''' 用于自动计算跳跃系数的高度（以像素为单位）
    ''' </summary>
    Const ReferenceHeight As Integer = 2560

    Public Function GetNextTap(image As Bitmap) As TapInformation Implements IGameRobot.GetNextTap
        Dim pair As PositionPair = Solve(image)
        Return New TapInformation(New Vector2(100, 100), pair.Distance * ReferenceHeight / image.Height)
    End Function

    Private Function Solve(image As Bitmap) As PositionPair
        Dim result As PositionPair
        Dim width = image.Width
        Dim height = image.Height
        Using pg = Graphics.FromImage(image)
            Dim offsetX As Integer = width * PercentOffsetX
            Dim offsetY As Integer = height * PercentOffsetY
            Dim uponX = width * PercentUponX
            Dim uponY = height * PercentUponY

            '生成聚类
            Dim clusters = GenerateClusters(image, offsetX, offsetY, uponX, uponY)
            Dim cluster1 As Cluster = clusters.Item1
            Dim cluster2 As Cluster = clusters.Item2

            '聚类中心(目标落点）
            Dim center1 = cluster1.GetCenter()
            Console.WriteLine($"cluster1:{cluster1.Vertices.Count}")
            '聚类中心(小人位置）
            Dim center2 = cluster2.GetCenter() + New Vector2(0, height * PercentCharacterOffset)
            Console.WriteLine($"cluster2:{cluster2.Vertices.Count}")

            If Math.Abs(center1.X - center2.X) < 20 Then
                Throw New Exception
            End If



            '绘制聚类
            DrawCluster(image, Pens.Red, cluster1)
            'DrawCluster(image, Pens.Green, cluster2)
            '绘制小人与目标落点的连线
            pg.DrawLine(Pens.Red, center1.X, center1.Y, center2.X, center2.Y)
            '绘制聚类矩形框
            pg.DrawRectangle(New Pen(Color.FromArgb(100, 0, 0, 255), 4), cluster1.GetRect)
            pg.DrawRectangle(New Pen(Color.FromArgb(100, 0, 255, 0), 4), cluster2.GetRect)
            '绘制聚类中心
            Dim radius As Single = 4
            pg.FillEllipse(Brushes.Yellow, center1.X - radius, center1.Y - radius, radius * 2, radius * 2)
            pg.FillEllipse(Brushes.Yellow, center2.X - radius, center2.Y - radius, radius * 2, radius * 2)
            '绘制水平参考线
            pg.DrawLine(Pens.Black, 0, offsetY, width, offsetY)
            pg.DrawLine(Pens.Black, 0, uponY, width, uponY)
            '绘制垂直参考线
            pg.DrawLine(Pens.Black, offsetX, 0, offsetX, height)
            pg.DrawLine(Pens.Black, uponX, 0, uponX, height)

            result = New PositionPair(center1, center2)
        End Using

        Return result
    End Function

    ''' <summary>
    ''' 返回图像指定区域的聚类集合识别结果
    ''' </summary>
    Private Function GenerateClusters(image As Bitmap, offsetX As Integer, offsetY As Integer, uponX As Integer, uponY As Integer) As Tuple(Of Cluster, Cluster)

        '角色顶部颜色
        Dim characterColor = Color.FromArgb(255, 54, 57, 100)

        Dim width = image.Width
        Dim height = image.Height


        Dim targetsCount As Integer = 4
        Dim targetsOfBox As New List(Of Color)


        '落点的顶部位置
        Dim TopOfTargetBox As Vertex
        '角色的顶部位置
        Dim TopOfCharacter As Vertex

        Dim over1 As Boolean
        Dim over2 As Boolean

        '扫描行跳过角色头部
        Dim isAvoidHead As Boolean = False
        Dim avoidCenter As Vector2

        For j = offsetY To uponY
            For i = offsetX To uponX
                Dim current = image.GetPixel(i, j)
                If Not over1 Then
                    '搜索落点顶端像素
                    If isAvoidHead Then
                        If (avoidCenter - New Vector2(i, j)).Length < width * 0.05 Then
                            Continue For
                        End If
                    End If
                    If ColorHelper.CompareBaseRGB(current, image.GetPixel(i, j - 1), 10) = False Then
                        If ColorHelper.CompareBaseRGB(current, image.GetPixel(i - 1, j), 10) = False Then
                            If ColorHelper.CompareBaseRGB(characterColor, image.GetPixel(i, j), 10) = False AndAlso
                                ColorHelper.CompareBaseRGB(characterColor, image.GetPixel(i, j + 1), 20) = False AndAlso
                                ColorHelper.CompareBaseRGB(characterColor, image.GetPixel(i, j + 3), 30) = False Then
                                TopOfTargetBox = New Vertex(New Vector2(i, j + 3), image.GetPixel(i, j + 3))

                                For k = 0 To targetsCount - 1
                                    targetsOfBox.Add(image.GetPixel(i, j + 3 + 8 * k))
                                Next
                                over1 = True
                                If isAvoidHead Then
                                    over2 = True
                                End If
                            Else
                                isAvoidHead = True
                                avoidCenter = New Vector2(i, j) + New Vector2(0, width * 0.027)
                                TopOfCharacter = New Vertex(New Vector2(i, j), current)
                            End If
                        End If
                    End If
                Else
                    '搜索小人顶端像素
                    If ColorHelper.CompareBaseRGB(current, characterColor, 25) = True Then
                        TopOfCharacter = New Vertex(New Vector2(i, j), current)
                        over2 = True
                    End If
                End If
                If over2 Then
                    Exit For
                End If
            Next
        Next

        Dim lowY1 = TopOfTargetBox.Position.Y
        Dim lowY2 = TopOfCharacter.Position.Y

        Dim uponY1 = TopOfTargetBox.Position.Y + height * PercentTargetBoxHeight
        Dim uponY2 = TopOfCharacter.Position.Y + height * PercentCharacterHeight

        uponY1 = If(uponY1 > uponY, uponY, uponY1)
        uponY2 = If(uponY2 > uponY, uponY, uponY2)
        If uponY1 > uponY2 Then uponY1 = uponY2


        Dim lowX1 = TopOfTargetBox.Position.X - width * PercentTargetBoxWidth / 2
        Dim lowX2 = TopOfCharacter.Position.X - width * PercentCharacterWidth / 2

        Dim uponX1 = TopOfTargetBox.Position.X + width * PercentTargetBoxWidth / 2
        Dim uponX2 = TopOfCharacter.Position.X + width * PercentCharacterWidth / 2

        uponX1 = If(uponX1 > uponX, uponX, uponX1)
        uponX2 = If(uponX2 > uponX, uponX, uponX2)




        '生成聚类
        'Dim cluster1 = GetCluster(image, TopOfTargetBox.Color, lowX1, uponX1, lowY1, uponY1, 6)
        Dim cluster1 = GetCluster(image, targetsOfBox.ToArray, lowX1, uponX1, lowY1, uponY1, 3)
        Dim cluster2 = GetCluster(image, TopOfCharacter.Color, lowX2, uponX2, lowY2, uponY2, 35)

        '移除盒子聚类中属于角色底部附近的点
        'Dim bottom = cluster2.GetCenter() + New Vector2(0, height * PercentCharacterOffset)
        'Dim radius = height * PercentCharacterHeight * 0.6
        'cluster1.Vertices.RemoveAll(Function(vertex) (vertex.Position - bottom).Length < radius)

        Return New Tuple(Of Cluster, Cluster)(cluster1, cluster2)
    End Function

    ''' <summary>
    ''' 返回指定区域内与指定颜色相似的聚类
    ''' </summary>
    Private Function GetCluster(image As Bitmap, target As Color, lowerX As Integer, uponX As Integer, lowerY As Integer, uponY As Integer, Optional distance As Integer = 20) As Cluster
        Dim result As New Cluster
        For j = lowerY To uponY
            For i = lowerX To uponX
                Dim current As Color = image.GetPixel(i, j)
                If ColorHelper.CompareBaseRGB(current, target, distance) Then
                    result.Vertices.Add(New Vertex(New Vector2(i, j), current))
                End If
            Next
        Next
        Return result
    End Function

    ''' <summary>
    ''' 返回指定区域内与指定颜色集合相似的聚类
    ''' </summary>
    Private Function GetCluster(image As Bitmap, targets As Color(), lowerX As Integer, uponX As Integer, lowerY As Integer, uponY As Integer, Optional distance As Integer = 20) As Cluster
        Dim result As New Cluster
        For j = lowerY To uponY
            For i = lowerX To uponX
                Dim current As Color = image.GetPixel(i, j)
                For k = 0 To targets.Length - 1
                    If ColorHelper.CompareBaseRGB(current, targets(k), distance) Then
                        result.Vertices.Add(New Vertex(New Vector2(i, j), current))
                        Exit For
                    End If
                Next
            Next
        Next
        Return result
    End Function

    ''' <summary>
    ''' 绘制聚类
    ''' </summary>
    Private Sub DrawCluster(image As Bitmap, pen As Pen, cluster As Cluster)
        If (cluster.Vertices.Count > 0) Then
            Using pg = Graphics.FromImage(image)
                For i = 0 To cluster.Vertices.Count - 1
                    Dim position = cluster.Vertices(i).Position
                    pg.DrawRectangle(pen, position.X, position.Y, 1, 1)
                Next
            End Using
        End If
    End Sub

End Class
