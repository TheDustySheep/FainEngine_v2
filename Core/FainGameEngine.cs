using FainEngine_v2.Rendering.Cameras;
using FainEngine_v2.Resources;
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
        window = Window.Create(options);

        window.Load += OnLoad;
        window.Update += OnUpdate;
        window.Render += OnRender;
        window.Closing += OnClose;
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

    private void OnLoad()
    {
        _gl = GL.GetApi(window);


        _gl.ClearColor(52.9f / 100f, 80.8f / 100f, 92.2f / 100f, 0);

        _gl.Enable(EnableCap.CullFace);
        _gl.FrontFace(FrontFaceDirection.CW);
        _gl.CullFace(TriangleFace.Back);
        _gl.Enable(EnableCap.Multisample);

        GameGraphics.SetGL(_gl);
        ResourceLoader.SetGL(_gl);
        Gizmos.Init(_gl);
        GameInputs.SetWindow(window);
        ICamera.SetWindow(window);
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

        GameInputs.Reset();

        Gizmos.Tick();
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
