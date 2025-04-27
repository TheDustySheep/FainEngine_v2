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

namespace FainEngine_v2.Core;
public static class GameGraphics
{
    public static event Action<int, int>? OnResized;
    public static void OnResize(Vector2D<int> newSize) => OnResized?.Invoke(newSize.X, newSize.Y);

    static IWindow? _window;
    public static IWindow Window => _window ?? throw new Exception("Window Not Set");

    public static float WindowAspect => Window.FramebufferSize.X / (float)Window.FramebufferSize.Y;

    private static GL? _gl;
    public static GL GL => _gl ?? throw new Exception("OpenGL Not Set");

    readonly static Dictionary<Material, List<RenderInstance>> RenderQueue = new();

    static PostProcess? _postProcess;

    static UIManager? UIManager;

    internal static void SetGL(GL gl, IWindow window)
    {
        _gl = gl;
        _window = window;

        UIManager = new();
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

        int totalMeshes = 0;
        int renderedMeshes = 0;

        foreach ((var mat, var queue) in RenderQueue)
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
                totalMeshes++;
                if (mesh.ClipBounds && !frustum.Intersects(bounds))
                    continue;
                renderedMeshes++;

                mat.SetModelMatrix(modelMatrix);

                mesh.Bind();
                mesh.Draw();
            }
        }
        RenderQueue.Clear();

        // TODO Render Transparent

        // Draw onto the main screen
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        _postProcess?.Draw();

        // Draw UI
        UIManager?.Draw();
    }

    public static void DrawMesh(IMesh mesh, Material mat, Matrix4x4 model)
    {
        if (!RenderQueue.TryGetValue(mat, out var queue))
        {
            queue = new List<RenderInstance>();
            RenderQueue.Add(mat, queue);
        }

        queue.Add(new RenderInstance()
        {
            Mesh = mesh,
            ModelMatrix = model,
        });
    }

    public static void SetPostProcess(PostProcess postProcess)
    {
        _postProcess = postProcess;
    }

    private struct RenderInstance
    {
        public required IMesh Mesh;
        public required Matrix4x4 ModelMatrix;
    }
}
