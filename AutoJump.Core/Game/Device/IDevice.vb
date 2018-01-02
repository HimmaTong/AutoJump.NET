Imports System.Drawing
Imports System.Numerics
''' <summary>
''' 设备接口
''' </summary>
Public Interface IDevice
    ''' <summary>
    ''' 设备是否生效
    ''' </summary>
    ReadOnly Property Availiable As Boolean
    ''' <summary>
    ''' 获取此设备的屏幕大小（以像素为单位）
    ''' </summary>
    ReadOnly Property Size As Size
    ''' <summary>
    ''' 按压
    ''' </summary>
    Sub Press(position As Vector2, millionseconds As Integer)
    ''' <summary>
    ''' 获取设备屏幕图像
    ''' </summary>
    Function GetScreenImage() As Image
End Interface
