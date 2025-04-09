using Avalonia.Controls;
using Avalonia.Controls.Platform;
using Avalonia.Platform;
using Silk.NET.GLFW;
using Silk.NET.Maths;
using Silk.NET.Windowing;
using System;

public class SilkHostVulkan : NativeControlHost
{
    private SilkControlVulkan _silkControlVulkan;
    private IPlatformHandle _platformHandle;

    protected override IPlatformHandle CreateNativeControlCore(IPlatformHandle parent)
    {
        // 获取父窗口句柄
        var parentHandle = parent.Handle;

        // 创建 Silk.NET 控件
        _silkControlVulkan = new SilkControlVulkan();

        // 获取子窗口句柄（需要扩展方法）
        var childHandle = _silkControlVulkan._glfwNativeWindow?.Win32?.Hwnd;

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
        _silkControlVulkan?.Dispose();
        base.DestroyNativeControlCore(control);
    }

    protected override void OnSizeChanged(SizeChangedEventArgs e)
    {
        var scaling = TopLevel.GetTopLevel(this).RenderScaling;

        int renderWidth = (int)(Bounds.Width * scaling);
        int renderHeight = (int)(Bounds.Height * scaling);

        if (_silkControlVulkan != null)
        {
            if (_silkControlVulkan._window.Size.X != renderWidth || _silkControlVulkan._window.Size.Y != renderHeight)
            {
                _silkControlVulkan._window.Size = new Vector2D<int>(renderWidth, renderHeight);
            }
        }
       
        base.OnSizeChanged(e);
    }

    public void Render() => _silkControlVulkan?.Render();
}