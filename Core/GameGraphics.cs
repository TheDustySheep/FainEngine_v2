using FainEngine_v2.Rendering.Cameras;
using FainEngine_v2.Rendering.Materials;
using FainEngine_v2.Rendering.Meshing;
using Silk.NET.OpenGL;
using System.Numerics;

namespace FainEngine_v2.Core;
public static class GameGraphics
{
    static readonly List<RenderInstance> renderQueue = new();
    private static GL? _gl;
    public static GL GL => _gl ?? throw new Exception("OpenGL Not Set");

    internal static void SetGL(GL gl)
    {
        _gl = gl;
    }

    public static void Render()
    {
        GL.Enable(EnableCap.DepthTest);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        ICamera camera = ICamera.Main;

        foreach (var render in renderQueue)
        {
            render.mesh.Bind();

            var mat = render.material;
            mat.Use();
            mat.SetProjectionMatrix(camera.ProjectionMatrix);
            mat.SetViewMatrix(camera.ViewMatrix);
            mat.SetModelMatrix(render.model);

            render.mesh.Draw();
        }
        renderQueue.Clear();
    }

    public static void DrawMesh(IMesh mesh, Material material, Matrix4x4 model)
    {
        renderQueue.Add(new RenderInstance()
        {
            mesh = mesh,
            material = material,
            model = model,
        });
    }

    private struct RenderInstance
    {
        public required IMesh mesh;
        public required Material material;
        public required Matrix4x4 model;
    }
}
