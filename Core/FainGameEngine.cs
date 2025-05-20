using FainEngine_v2.Rendering;
using FainEngine_v2.Resources;
using FainEngine_v2.Utils;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace FainEngine_v2.Core;
public class FainGameEngine
{
    private readonly IWindow window;
    private GL? _gl;

    public FainGameEngine(
        int windowWidth = 1600,
        int windowHeight = 900,
        string windowTitle = "New Game Engine")
    {
        var options = WindowOptions.Default;
        options.Size = new Vector2D<int>(windowWidth, windowHeight);
        options.Title = windowTitle;
        options.Samples = 4;
        options.PreferredDepthBufferBits = 24;
        options.VSync = false;

        options.API = new GraphicsAPI(
            ContextAPI.OpenGL,
            ContextProfile.Core,
            ContextFlags.ForwardCompatible,
            new APIVersion(4, 6));
        
        window = Window.Create(options);

        window.Load += OnLoad;
        window.Update += OnUpdate;
        window.Render += OnRender;
        window.Closing += OnClose;
        window.FramebufferResize += OnResize;
    }

    public void Run()
    {
        window.Run();
        window.Dispose();
    }

    protected virtual void Load() { }
    protected virtual void Update() { }
    protected virtual void FixedUpdate() { }
    protected virtual void Render() { }
    protected virtual void Close() { }

    private void OnResize(Vector2D<int> newSize)
    {
        _gl?.Viewport(newSize);
        GameGraphics.OnResize(newSize);
    }

    private void OnLoad()
    {
        _gl = GL.GetApi(window);

        _gl.Enable(EnableCap.CullFace);
        _gl.FrontFace(FrontFaceDirection.CW);
        _gl.CullFace(TriangleFace.Back);

        _gl.Enable(EnableCap.Multisample);

        GameGraphics.SetGL(_gl, window);
        ResourceLoader.SetGL(_gl);
        Gizmos.Init();
        GameInputs.SetWindow(window);
        Load();
    }

    private void OnUpdate(double deltaTime)
    {
        GameTime.Tick((float)deltaTime);

        // Fixed update loop
        while (GameTime.TickFixedUpdate())
        {
            EntityManager.FixedUpdate();
            FixedUpdate();
        }

        // Update loop
        EntityManager.Update();
        Update();

        MainThreadDispatcher.ExecutePending();

        Gizmos.Tick();

        GameInputs.Reset();
    }

    private void OnRender(double deltaTime)
    {
        if (_gl is null)
            return;

        GameGraphics.Render();
        Render();
    }

    private void OnClose()
    {
        EntityManager.Dispose();
        Close();
    }
}
