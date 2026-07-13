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
public class GameGraphics : IGameGraphics
{
    #region Internal
    private readonly GL _GL;
    private readonly IWindow _window;
    #endregion

    #region Public Variables
    public event Action<int, int>? OnResized;
    public void OnResize(Vector2D<int> newSize) => OnResized?.Invoke(newSize.X, newSize.Y);
    public float WindowAspect => _window.FramebufferSize.X / (float)_window.FramebufferSize.Y;
    #endregion

    private readonly RenderQueue renderQueue = new RenderQueue();
    private PostProcess? _postProcess;
    //private static readonly HashSet<UIController> UIControllers = new();

    private readonly ILightingController lightingController = new LightingController();

    public GameGraphics(GL gl, IWindow window)
    {
        _GL = gl;
        _window = window;
    }

    public void Render()
    {

        // Bind Frame Buffer
        _postProcess?.Bind();
        ThrowOnGLError("Binding Post process");

        // Render Opaque
        _GL.Enable(EnableCap.CullFace);
        _GL.Enable(EnableCap.DepthTest);
        _GL.ClearColor(52.9f / 100f, 80.8f / 100f, 92.2f / 100f, 0);
        _GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        _GL.DepthFunc(DepthFunction.Less);
        _GL.DepthMask(true);
        ThrowOnGLError("Setting all the starting stuff");


        ICamera camera = ICamera.Main;
        Frustum frustum = camera.Frustum;

        DrawCallDebugData debug = new();

        // Render Opaque
        ThrowOnGLError("Pre-Opaque Pass");
        debug.OpaqueCalls = RenderPass(camera, frustum, renderQueue.Opaque());
        ThrowOnGLError("Post-Opaque Pass");

        // Render Transparent
        _GL.DepthMask(false);
        _GL.Enable(EnableCap.Blend);
        _GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        ThrowOnGLError("Pre-Transparent Pass");
        debug.TransparentCalls = RenderPass(camera, frustum, renderQueue.Transparent());
        ThrowOnGLError("Post-Transparent Pass");
        _GL.Disable(EnableCap.Blend);
        _GL.DepthMask(true);

        // GenerateVertices onto the main screen
        ThrowOnGLError("Pre-Clear framebuffer");
        _GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        _postProcess?.Draw();
        ThrowOnGLError("Post-Processing Drawing");
        _GL.Clear(ClearBufferMask.DepthBufferBit);
        _GL.DepthMask(true);

        // GenerateVertices OldUI
        _GL.Disable(EnableCap.CullFace);
        _GL.Disable(EnableCap.DepthTest);
        _GL.Enable(EnableCap.Blend);
        _GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

        ThrowOnGLError("Pre-Overlay");
        debug.UICalls = OverlayRenderPass(renderQueue.Overlay());
        ThrowOnGLError("Post-Overlay");
        _GL.Disable(EnableCap.Blend);

        RenderDebugVariables.DrawCallDebugData.Value = debug;

        // Clear the render Queue
        renderQueue.Clear();
    }

    public void ThrowOnGLError(string details)
    {
        var error = _GL.GetError();
        if (error == GLEnum.NoError)
            return;

        throw new Exception($"OpenGL Error [{error}] Details: {details}");
    }

    private uint OverlayRenderPass(Dictionary<Material, List<RenderInstance>> pass)
    {
        var cameraProjection = Matrix4x4.CreateOrthographicOffCenter(
            0,
            _window.Size.X,
            _window.Size.Y,
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

    private uint RenderPass(ICamera camera, Frustum frustum, Dictionary<Material, List<RenderInstance>> pass)
    {
        uint drawCalls = 0;
        foreach ((var mat, var queue) in pass)
        {
            mat.Use();

            if (_postProcess != null)
                mat.SetRenderTexture(_postProcess._rt);

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

    public void DrawMesh(IMesh mesh, Material mat, Matrix4x4 model)
    {
        renderQueue.Enqueue(mesh, mat, model);
    }

    public void SetPostProcess(PostProcess postProcess)
    {
        _postProcess = postProcess;
    }
}