using Avalonia.Controls;
using Avalonia.Controls.Platform;
using Avalonia.Platform;
using Silk.NET.GLFW;
using Silk.NET.Maths;
using Silk.NET.Windowing;
using System;

public class SilkHostDirectX : NativeControlHost
{
    private SilkControlDirectX _silkControlDirectX;
    //private IPlatformHandle _platformHandle;

    protected override IPlatformHandle CreateNativeControlCore(IPlatformHandle parent)
    {
        // 获取父窗口句柄
        var parentHandle = parent.Handle;

        // 创建 Silk.NET 控件
        _silkControlDirectX = new SilkControlDirectX();

        // 获取子窗口句柄（需要扩展方法）
        var childHandle = _silkControlDirectX._glfwNativeWindow?.Win32?.Hwnd;

        //Console.WriteLine(childHandle.ToString());

        // 根据平台返回句柄
        return GetPlatformHandle((nint)childHandle);
    }

    private IPlatformHandle GetPlatformHandle(IntPtr handle)
    {
        if (OperatingSystem.IsWindows())
            return new PlatformHandle(handle, "HWND");
        else if (OperatingSystem.IsLinux())
            return new PlatformHandle(handle, "XID");
        else if (OperatingSystem.IsMacOS())
            return new PlatformHandle(handle, "NSView");
        else
            throw new PlatformNotSupportedException();
    }

    protected override void DestroyNativeControlCore(IPlatformHandle control)
    {
        _silkControlDirectX?.Dispose();
        base.DestroyNativeControlCore(control);
    }

    protected override void OnSizeChanged(SizeChangedEventArgs e)
    {
        var scaling = TopLevel.GetTopLevel(this).RenderScaling;

        int renderWidth = (int)(Bounds.Width * scaling);
        int renderHeight = (int)(Bounds.Height * scaling);

        if (_silkControlDirectX != null)
        {
            if (_silkControlDirectX._window.Size.X != renderWidth || _silkControlDirectX._window.Size.Y != renderHeight)
            {
                _silkControlDirectX._window.Size = new Vector2D<int>(renderWidth, renderHeight);
            }
        }
       
        base.OnSizeChanged(e);
    }

    public void Render() => _silkControlDirectX?.Render();
}