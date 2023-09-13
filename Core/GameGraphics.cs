using FainEngine_v2.Rendering.BoundingShapes;
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
        Frustum frustum = camera.Frustum;

        int totalMeshes = 0;
        int renderedMeshes = 0;

        foreach (var render in renderQueue)
        {
            IMesh mesh = render.Mesh;
            Matrix4x4 modelMatrix = render.ModelMatrix;
            BoundingBox bounds = mesh.Bounds;
            bounds = bounds.Transform(modelMatrix);

            totalMeshes++;

            if (!frustum.Intersects(bounds))
                continue;

            renderedMeshes++;

            render.Mesh.Bind();

            var mat = render.Material;
            mat.Use();
            mat.UpdateAdditionalUniforms();
            mat.SetProjectionMatrix(camera.ProjectionMatrix);
            mat.SetViewMatrix(camera.ViewMatrix);
            mat.SetModelMatrix(modelMatrix);

            render.Mesh.Draw();
        }
        renderQueue.Clear();
        Console.WriteLine($"Total Meshes: {totalMeshes} Rendered: {renderedMeshes}");
    }

    public static void DrawMesh(IMesh mesh, Material material, Matrix4x4 model)
    {
        renderQueue.Add(new RenderInstance()
        {
            Mesh = mesh,
            Material = material,
            ModelMatrix = model,
        });
    }

    private struct RenderInstance
    {
        public required IMesh Mesh;
        public required Material Material;
        public required Matrix4x4 ModelMatrix;
    }
}
