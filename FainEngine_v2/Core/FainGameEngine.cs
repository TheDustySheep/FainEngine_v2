using FainEngine_v2.Rendering;
using FainEngine_v2.Resources;
using FainEngine_v2.Utils;
using FainEngine_v2.Utils.Threading;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using System;

namespace FainEngine_v2.Core;
public class FainGameEngine
{
    protected readonly IWindow window;
    protected GameTime? gameTime;
    protected GameInputs? gameInputs;
    protected GameGraphics? graphics;

    private GL? _GL;

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
        DependencyInjector.RegisterSingleton(window);

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
        _GL?.Viewport(newSize);
        graphics?.OnResize(newSize);
    }

    private void OnLoad()
    {
        _GL = GL.GetApi(window);
        DependencyInjector.RegisterSingleton(_GL);

        gameTime = new GameTime();
        gameInputs = new GameInputs(window);

        DependencyInjector.RegisterSingleton<IGameTime>(gameTime);
        DependencyInjector.RegisterSingleton<IGameInputs>(gameInputs);

        // Register Graphics
        graphics = new GameGraphics(_GL, window);
        DependencyInjector.RegisterSingleton<IGameGraphics>(graphics);

        // GL Setup
        _GL.Enable(EnableCap.CullFace);
        _GL.FrontFace(FrontFaceDirection.CW);
        _GL.CullFace(TriangleFace.Back);

        _GL.Enable(EnableCap.Multisample);

        ResourceLoader.SetGL(_GL);
        GLDisposalService.Init(_GL);

        Gizmos.Init();
        Load();
    }

    private void OnUpdate(double deltaTime)
    {
        gameTime?.Tick((float)deltaTime);

        MainThreadDispatcher.ExecutePending();

        // Fixed update loop
        while (gameTime != null && gameTime.TickFixedUpdate())
            FixedUpdate();

        // Draw loop
        Update();

        Gizmos.Update();

        gameInputs?.Reset();

        GLDisposalService.Collect();
    }

    private void OnRender(double deltaTime)
    {
        if (_GL is null)
            return;

        graphics?.ThrowOnGLError("Start Game Engine Loop");
        graphics?.Render();
        Render();
        graphics?.ThrowOnGLError("End Game Engine Render");
    }

    private void OnClose()
    {
        Close();
    }
}
