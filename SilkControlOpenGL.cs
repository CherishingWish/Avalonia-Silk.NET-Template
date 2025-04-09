using System;
using Silk.NET.Core.Contexts;
using Silk.NET.Core.Native;
using Silk.NET.GLFW;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using Silk.NET.Windowing.Glfw;

public class SilkControlOpenGL : IDisposable
{
    public IWindow _window;
    private GL Gl;

    public Glfw? _glfw = null;
    public GlfwNativeWindow? _glfwNativeWindow = null;

    private static uint Vbo;
    private static uint Ebo;
    private static uint Vao;
    private static uint Shader;

    //Vertex shaders are run on each vertex.
    private static readonly string VertexShaderSource = @"
        #version 330 core //Using version GLSL version 3.3
        layout (location = 0) in vec4 vPos;
        
        void main()
        {
            gl_Position = vec4(vPos.x, vPos.y, vPos.z, 1.0);
        }
        ";

    //Fragment shaders are run on each fragment/pixel of the geometry.
    private static readonly string FragmentShaderSource = @"
        #version 330 core
        out vec4 FragColor;

        void main()
        {
            FragColor = vec4(1.0f, 0.5f, 0.2f, 1.0f);
        }
        ";

    //Vertex data, uploaded to the VBO.
    private static readonly float[] Vertices =
    {
            //X    Y      Z
             0.5f,  0.5f, 0.0f,
             0.5f, -0.5f, 0.0f,
            -0.5f, -0.5f, 0.0f,
            -0.5f,  0.5f, 0.5f
        };

    //Index data, uploaded to the EBO.
    private static readonly uint[] Indices =
    {
            0, 1, 3,
            1, 2, 3
        };

    public unsafe SilkControlOpenGL()
    {
        // 创建窗口配置
        var options = WindowOptions.Default;
        options.ShouldSwapAutomatically = true;
        options.IsVisible = false; // 初始不可见
        options.WindowClass = "SilkHost";
        options.WindowBorder = WindowBorder.Hidden;

        // 创建窗口
        _window = Window.Create(options);
        _glfw = GlfwWindowing.GetExistingApi(_window);

        _window.FramebufferResize += OnResize;
        _window.Initialize();

        _glfwNativeWindow = new GlfwNativeWindow(_glfw, (WindowHandle*)_window.Handle);

        // 初始化 OpenGL
        Gl = GL.GetApi(_window);

        //string renderer = SilkMarshal.PtrToString((nint)Gl.GetString(StringName.Renderer));
        //string vendor = SilkMarshal.PtrToString((nint)Gl.GetString(StringName.Vendor));

        //Console.WriteLine($"OpenGL渲染器: {renderer}");
        //Console.WriteLine($"显卡供应商: {vendor}");

        //Creating a vertex array.
        Vao = Gl.GenVertexArray();
        Gl.BindVertexArray(Vao);

        //Initializing a vertex buffer that holds the vertex data.
        Vbo = Gl.GenBuffer(); //Creating the buffer.
        Gl.BindBuffer(BufferTargetARB.ArrayBuffer, Vbo); //Binding the buffer.
        fixed (void* v = &Vertices[0])
        {
            Gl.BufferData(BufferTargetARB.ArrayBuffer, (nuint)(Vertices.Length * sizeof(uint)), v, BufferUsageARB.StaticDraw); //Setting buffer data.
        }

        //Initializing a element buffer that holds the index data.
        Ebo = Gl.GenBuffer(); //Creating the buffer.
        Gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, Ebo); //Binding the buffer.
        fixed (void* i = &Indices[0])
        {
            Gl.BufferData(BufferTargetARB.ElementArrayBuffer, (nuint)(Indices.Length * sizeof(uint)), i, BufferUsageARB.StaticDraw); //Setting buffer data.
        }

        //Creating a vertex shader.
        uint vertexShader = Gl.CreateShader(ShaderType.VertexShader);
        Gl.ShaderSource(vertexShader, VertexShaderSource);
        Gl.CompileShader(vertexShader);

        //Checking the shader for compilation errors.
        string infoLog = Gl.GetShaderInfoLog(vertexShader);
        if (!string.IsNullOrWhiteSpace(infoLog))
        {
            Console.WriteLine($"Error compiling vertex shader {infoLog}");
        }

        //Creating a fragment shader.
        uint fragmentShader = Gl.CreateShader(ShaderType.FragmentShader);
        Gl.ShaderSource(fragmentShader, FragmentShaderSource);
        Gl.CompileShader(fragmentShader);

        //Checking the shader for compilation errors.
        infoLog = Gl.GetShaderInfoLog(fragmentShader);
        if (!string.IsNullOrWhiteSpace(infoLog))
        {
            Console.WriteLine($"Error compiling fragment shader {infoLog}");
        }

        //Combining the shaders under one shader program.
        Shader = Gl.CreateProgram();
        Gl.AttachShader(Shader, vertexShader);
        Gl.AttachShader(Shader, fragmentShader);
        Gl.LinkProgram(Shader);

        //Checking the linking for errors.
        Gl.GetProgram(Shader, GLEnum.LinkStatus, out var status);
        if (status == 0)
        {
            Console.WriteLine($"Error linking shader {Gl.GetProgramInfoLog(Shader)}");
        }

        //Delete the no longer useful individual shaders;
        Gl.DetachShader(Shader, vertexShader);
        Gl.DetachShader(Shader, fragmentShader);
        Gl.DeleteShader(vertexShader);
        Gl.DeleteShader(fragmentShader);

        //Tell opengl how to give the data to the shaders.
        Gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), null);
        Gl.EnableVertexAttribArray(0);
    }

    public unsafe void Render()
    {
        //Console.WriteLine("Hello!");

        //Clear the color channel.
        Gl.Clear((uint)ClearBufferMask.ColorBufferBit);

        //Bind the geometry and shader.
        Gl.BindVertexArray(Vao);
        Gl.UseProgram(Shader);

        //Draw the geometry.
        Gl.DrawElements(PrimitiveType.Triangles, (uint)Indices.Length, DrawElementsType.UnsignedInt, null);

        _window.DoEvents();
        _window.SwapBuffers();
    }

    public void OnResize(Vector2D<int> newSize)
    {
        //Console.WriteLine("Hello");
        Gl.Viewport(0, 0, (uint)newSize.X, (uint)newSize.Y);
    }

    public void Dispose()
    {
        Gl?.Dispose();
        _window?.Dispose();    
    }
}
