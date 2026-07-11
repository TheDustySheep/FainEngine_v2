using FainEngine_v2.Entities;
using FainEngine_v2.Rendering.BoundingShapes;
using FainEngine_v2.Rendering.Cameras;
using FainEngine_v2.Rendering.Lighting;
using FainEngine_v2.Rendering.Materials;
using FainEngine_v2.Rendering.Meshing;
using FainEngine_v2.Rendering.PostProcessing;
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

    private static IWindow? _window;
    public static IWindow Window => _window ?? throw new Exception("Window Not Set");

    public static float WindowAspect => Window.FramebufferSize.X / (float)Window.FramebufferSize.Y;
    #endregion

    private readonly static RenderQueue renderQueue = new RenderQueue();
    private static PostProcess? _postProcess;
    //private static readonly HashSet<UIController> UIControllers = new();

    private static readonly ILightingController lightingController = new LightingController();

    internal static void SetGL(GL gl, IWindow window)
    {
        _gl = gl;
        _window = window;
    }

    public static void Render()
    {

        // Bind Frame Buffer
        _postProcess?.Bind();
        ThrowOnGLError("Binding Post process");

        // Render Opaque
        GL.Enable(EnableCap.CullFace);
        GL.Enable(EnableCap.DepthTest);
        GL.ClearColor(52.9f / 100f, 80.8f / 100f, 92.2f / 100f, 0);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        GL.DepthFunc(DepthFunction.Less);
        GL.DepthMask(true);
        ThrowOnGLError("Setting all the starting stuff");


        ICamera camera = ICamera.Main;
        Frustum frustum = camera.Frustum;

        DrawCallDebugData debug = new();

        // Render Opaque
        ThrowOnGLError("Pre-Opaque Pass");
        debug.OpaqueCalls = RenderPass(camera, frustum, renderQueue.Opaque());
        ThrowOnGLError("Post-Opaque Pass");

        // Render Transparent
        GL.DepthMask(false);
        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        ThrowOnGLError("Pre-Transparent Pass");
        debug.TransparentCalls = RenderPass(camera, frustum, renderQueue.Transparent());
        ThrowOnGLError("Post-Transparent Pass");
        GL.Disable(EnableCap.Blend);
        GL.DepthMask(true);

        // GenerateVertices onto the main screen
        ThrowOnGLError("Pre-Clear framebuffer");
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        _postProcess?.Draw();
        ThrowOnGLError("Post-Processing Drawing");
        GL.Clear(ClearBufferMask.DepthBufferBit); 
        GL.DepthMask(true);

        // GenerateVertices OldUI
        GL.Disable(EnableCap.CullFace);
        GL.Disable(EnableCap.DepthTest);
        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

        ThrowOnGLError("Pre-Overlay");
        debug.UICalls = OverlayRenderPass(renderQueue.Overlay());
        ThrowOnGLError("Post-Overlay");
        GL.Disable(EnableCap.Blend);

        RenderDebugVariables.DrawCallDebugData.Value = debug;

        // Clear the render Queue
        renderQueue.Clear();
    }

    public static void ThrowOnGLError(string details)
    {
        var error = GL.GetError();
        if (error == GLEnum.NoError)
            return;

        throw new Exception($"OpenGL Error [{error}] Details: {details}");
    }

    private static uint OverlayRenderPass(Dictionary<Material, List<RenderInstance>> pass)
    {
        var cameraProjection = Matrix4x4.CreateOrthographicOffCenter(
            0, 
            Window.Size.X,
            Window.Size.Y,
            0,
            -1000, 
            1000
        );

        uint drawCalls = 0;
        foreach ((var mat, var queue) in pass)
        {
            mat.Use();
            mat.SetUniforms();
            mat.SetProjectionMatrix(cameraProjection);

            foreach (var render in queue)
            {
                IMesh mesh = render.Mesh;
                Matrix4x4 modelMatrix = render.ModelMatrix;
                mat.SetModelMatrix(modelMatrix);
                mesh.Draw();
                drawCalls++;
            }
        }

        return drawCalls;
    }

    private static uint RenderPass(ICamera camera, Frustum frustum, Dictionary<Material, List<RenderInstance>> pass)
    {
        uint drawCalls = 0;
        foreach ((var mat, var queue) in pass)
        {
            mat.Use();

            if (_postProcess != null)
                mat.SetRenderTexture(_postProcess.rt);

            mat.SetUniforms();
            mat.SetProjectionMatrix(camera.ProjectionMatrix);
            mat.SetLighting(lightingController);
            mat.SetViewMatrix(camera.ViewMatrix, camera);

            foreach (var render in queue)
            {
                IMesh mesh = render.Mesh;
                Matrix4x4 modelMatrix = render.ModelMatrix;
                BoundingBox bounds = mesh.Bounds.Transform(modelMatrix);

                // Skip meshes outside camera frustum
                if (mesh.ClipBounds && !frustum.Intersects(bounds))
                    continue;

                mat.SetModelMatrix(modelMatrix);

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

    //public static void RegisterController(UIController controller)
    //{
    //    UIControllers.Add(controller);
    //}
    //
    //public static void UnregisterController(UIController controller)
    //{
    //    UIControllers.Remove(controller);
    //}
}