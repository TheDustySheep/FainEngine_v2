using FainEngine_v2.Rendering.BoundingShapes;
using FainEngine_v2.Rendering.Cameras;
using FainEngine_v2.Rendering.Materials;
using FainEngine_v2.Rendering.Meshing;
using FainEngine_v2.UI;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using System.Numerics;

namespace FainEngine_v2.Core;
public static class GameGraphics
{
    static IWindow? _window;
    static IWindow Window => _window ?? throw new Exception("Window Not Set");

    public static float WindowAspect => Window.Size.X / (float)Window.Size.Y;

    private static GL? _gl;
    public static GL GL => _gl ?? throw new Exception("OpenGL Not Set");

    readonly static Dictionary<Material, List<RenderInstance>> RenderQueue = new();

    internal static void SetGL(GL gl, IWindow window)
    {
        _gl = gl;
        _window = window;
    }

    public static void Render()
    {
        GL.Enable(EnableCap.DepthTest);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        ICamera camera = ICamera.Main;
        Frustum frustum = camera.Frustum;

        int totalMeshes = 0;
        int renderedMeshes = 0;

        foreach ((var mat, var queue) in RenderQueue)
        {
            mat.Use();
            mat.UpdateAdditionalUniforms();
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
        //Console.WriteLine($"Total Meshes: {totalMeshes} Rendered: {renderedMeshes}");
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

    private struct RenderInstance
    {
        public required IMesh Mesh;
        public required Matrix4x4 ModelMatrix;
    }
}
