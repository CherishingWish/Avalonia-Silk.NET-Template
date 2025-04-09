using Avalonia.Controls;
using Avalonia.Threading;
using System;

namespace SilkTest;

public partial class MainWindow : Window
{
    private SilkHostDirectX _silkHostDirectX;
    private IDisposable _renderTimerDirectX;

    
    private SilkHostOpenGL _silkHostOpenGL;
    private IDisposable _renderTimerOpenGL;

    private SilkHostVulkan _silkHostVulkan;
    private IDisposable _renderTimerVulkan;


    public MainWindow()
    {
        InitializeComponent();

        // 创建 OpenTK 宿主控件
        _silkHostDirectX = new SilkHostDirectX();

        _renderTimerDirectX = DispatcherTimer.Run(() =>
        {
            _silkHostDirectX.Render();
            return true;
        }, TimeSpan.FromSeconds(1.0 / 60.0));

        MyContentControlDirectX.Content = _silkHostDirectX;

        // 创建 OpenTK 宿主控件
        
        _silkHostOpenGL = new SilkHostOpenGL();

        _renderTimerOpenGL = DispatcherTimer.Run(() =>
        {
            _silkHostOpenGL.Render();
            return true;
        }, TimeSpan.FromSeconds(1.0 / 60.0));

        MyContentControlOpenGL.Content = _silkHostOpenGL;

        // 创建 Vulkan 宿主控件

        _silkHostVulkan = new SilkHostVulkan();

        _renderTimerVulkan = DispatcherTimer.Run(() =>
        {
            _silkHostVulkan.Render();
            return true;
        }, TimeSpan.FromSeconds(1.0 / 60.0));

        MyContentControlVulkan.Content = _silkHostVulkan;

    }

    protected override void OnClosed(EventArgs e)
    {
        _renderTimerDirectX?.Dispose();
        _renderTimerOpenGL?.Dispose();
        _renderTimerVulkan?.Dispose();
        base.OnClosed(e);
    }
}