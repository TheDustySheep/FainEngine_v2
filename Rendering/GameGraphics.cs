using FainEngine_v2.Rendering.BoundingShapes;
using FainEngine_v2.Rendering.Cameras;
using FainEngine_v2.Rendering.Materials;
using FainEngine_v2.Rendering.Meshing;
using FainEngine_v2.Rendering.PostProcessing;
using FainEngine_v2.UI;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using System.Numerics;

namespace FainEngine_v2.Rendering;
public static class GameGraphics
{
    #region Public Variables
    public static event Action<int, int>? OnResized;
    public static void OnResize(Vector2D<int> newSize) => OnResized?.Invoke(newSize.X, newSize.Y);

    private static GL? _gl;
    public static GL GL => _gl ?? throw new Exception("OpenGL Not Set");
    #endregion

    static IWindow? _window;
    public static IWindow Window => _window ?? throw new Exception("Window Not Set");

    public static float WindowAspect => Window.FramebufferSize.X / (float)Window.FramebufferSize.Y;

    readonly static RenderQueue renderQueue = new RenderQueue();

    static PostProcess? _postProcess;

    static HashSet<UICanvas> Canvases = new();

    internal static void SetGL(GL gl, IWindow window)
    {
        _gl = gl;
        _window = window;
    }

    public static void Render()
    {
        // Bind Frame Buffer
        _postProcess?.Bind();

        // Render Opaque
        GL.Enable(EnableCap.DepthTest);
        GL.ClearColor(52.9f / 100f, 80.8f / 100f, 92.2f / 100f, 0);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        GL.DepthFunc(DepthFunction.Less);

        ICamera camera = ICamera.Main;
        Frustum frustum = camera.Frustum;

        DrawCallDebugData debug = new();

        debug.OpaqueCalls = RenderPass(camera, frustum, renderQueue.Opaque());

        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        debug.TransparentCalls = RenderPass(camera, frustum, renderQueue.Transparent());
        GL.Disable(EnableCap.Blend);

        renderQueue.Clear();

        // TODO Render Transparent

        // Draw onto the main screen
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        _postProcess?.Draw();

        // Draw UI
        foreach (var canvas in Canvases.OrderBy(i => i.Priority))
        {
            canvas.Draw();
            debug.UICalls++;
        }

        RenderDebugVariables.DrawCallDebugData.Value = debug;
    }

    private static uint RenderPass(ICamera camera, Frustum frustum, Dictionary<Material, List<RenderInstance>> pass)
    {
        uint drawCalls = 0;
        foreach ((var mat, var queue) in pass)
        {
            mat.Use();
            mat.SetUniforms();
            mat.SetProjectionMatrix(camera.ProjectionMatrix);
            mat.SetViewMatrix(camera.ViewMatrix);

            foreach (var render in queue)
            {
                IMesh mesh = render.Mesh;
                Matrix4x4 modelMatrix = render.ModelMatrix;
                BoundingBox bounds = mesh.Bounds.Transform(modelMatrix);

                // Skip meshes outside camera frustum
                if (mesh.ClipBounds && !frustum.Intersects(bounds))
                    continue;

                mat.SetModelMatrix(modelMatrix);

                mesh.Bind();
                mesh.Draw();
                drawCalls++;
            }
        }
        return drawCalls;
    }

    public static void DrawMesh(IMesh mesh, Material mat, Matrix4x4 model)
    {
        renderQueue.Enqueue(mesh, mat, model);
    }

    public static void SetPostProcess(PostProcess postProcess)
    {
        _postProcess = postProcess;
    }

    public static void RegisterCanvas(UICanvas canvas)
    {
        Canvases.Add(canvas);
    }

    public static void UnregisterCanvas(UICanvas canvas)
    {
        Canvases.Remove(canvas);
    }
}