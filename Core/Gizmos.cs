using FainEngine_v2.Rendering.Materials;
using FainEngine_v2.Rendering.Meshing;
using FainEngine_v2.Resources;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using System.Numerics;
using Color = System.Drawing.Color;

namespace FainEngine_v2.Core;
public static class Gizmos
{
    private struct Vertex
    {
        public Vector3 Position;
        public Vector3 Color;
    }

    class GizmoMesh : CustomVertexMesh<Vertex, uint>
    {
        public GizmoMesh(GL gl) : base(gl)
        {
            Bounds = new Rendering.BoundingShapes.BoundingBox()
            {
                Max = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue),
                Min = new Vector3(float.MinValue, float.MinValue, float.MinValue)
            };
        }

        public override unsafe void Draw()
        {
            if (Triangles is null || Triangles.Length == 0 || VertexCount == 0)
                return;

            VAO.Bind();
            _gl.PolygonMode(TriangleFace.FrontAndBack, PolygonMode.Line);
            _gl.DrawElements(PrimitiveType.Triangles, (uint)Triangles.Length, DrawElementsType.UnsignedInt, (void*)0);
            _gl.PolygonMode(TriangleFace.FrontAndBack, PolygonMode.Fill);
        }

        protected override Vector3 GetVertexPosition(Vertex vertex)
        {
            return vertex.Position;
        }
    }

    static readonly List<Vertex> Vertices = new();
    static readonly List<uint> Triangles = new();
    static GizmoMesh? Mesh;

    static Material? material;

    internal static void Init(GL gl)
    {
        Mesh = new GizmoMesh(gl);
        material = new Material(ResourceLoader.LoadShader("Resources/GizmoShader"));
    }

    internal static void Tick()
    {
        if (Mesh is null || material is null)
            return;

        Mesh.SetVertices(Vertices.ToArray());
        Mesh.SetTriangles(Triangles.ToArray());
        Mesh.Apply();

        GameGraphics.DrawMesh(Mesh, material, Matrix4x4.Identity);

        Vertices.Clear();
        Triangles.Clear();
    }

    private static Vector3 ConvertColor(Color color)
    {
        return new Vector3(color.R / 255f, color.G / 255f, color.B / 255f);
    }

    #region Bounds Gizmo
    public static void DrawBounds(Bounds bounds, Color color)
    {
        Vector3 _color = ConvertColor(color);

        Matrix4x4 mat =
           Matrix4x4.CreateScale(bounds.Size) *
           Matrix4x4.CreateTranslation(bounds.StartPos);

        uint vertCount = (uint)Vertices.Count;
        for (int face = 0; face < 6; face++)
        {
            for (int vertIndex = 0; vertIndex < 4; vertIndex++)
            {
                Vertex vert = VOXEL_VERTICES[face * 4 + vertIndex];
                vert.Position = Vector3.Transform(vert.Position, mat);
                vert.Color = _color;
                Vertices.Add(vert);
            }

            Triangles.Add(vertCount + 0);
            Triangles.Add(vertCount + 1);
            Triangles.Add(vertCount + 2);
            Triangles.Add(vertCount + 2);
            Triangles.Add(vertCount + 3);
            Triangles.Add(vertCount + 0);

            vertCount += 4;
        }
    }
    #endregion

    #region Voxel Gizmo
    public static void DrawVoxel(Vector3D<int> pos, Color color)
    {
        Vector3 _color = ConvertColor(color);

        float offset = 0.001f;

        Matrix4x4 mat =
            Matrix4x4.CreateScale(1 + (offset * 2)) *
            Matrix4x4.CreateTranslation((Vector3)pos + (Vector3.One * -offset));

        uint vertCount = (uint)Vertices.Count;
        for (int face = 0; face < 6; face++)
        {
            for (int vertIndex = 0; vertIndex < 4; vertIndex++)
            {
                Vertex vert = VOXEL_VERTICES[face * 4 + vertIndex];
                vert.Position = Vector3.Transform(vert.Position, mat);
                vert.Color = _color;
                Vertices.Add(vert);
            }

            Triangles.Add(vertCount + 0);
            Triangles.Add(vertCount + 1);
            Triangles.Add(vertCount + 2);
            Triangles.Add(vertCount + 2);
            Triangles.Add(vertCount + 3);
            Triangles.Add(vertCount + 0);

            vertCount += 4;
        }
    }

    static readonly Vertex[] VOXEL_VERTICES =
    {
        // XPos_px-
        new Vertex { Position = new Vector3(0, 0, 1) },
        new Vertex { Position = new Vector3(0, 1, 1) },
        new Vertex { Position = new Vector3(0, 1, 0) },
        new Vertex { Position = new Vector3(0, 0, 0) },

        // XPos_px+
        new Vertex { Position = new Vector3(1, 0, 0) },
        new Vertex { Position = new Vector3(1, 1, 0) },
        new Vertex { Position = new Vector3(1, 1, 1) },
        new Vertex { Position = new Vector3(1, 0, 1) },
                                                         
        // YPox_px-                                            
        new Vertex { Position = new Vector3(1, 0, 0) },
        new Vertex { Position = new Vector3(1, 0, 1) },
        new Vertex { Position = new Vector3(0, 0, 1) },
        new Vertex { Position = new Vector3(0, 0, 0) },
                                                         
        // YPox_px+                                            
        new Vertex { Position = new Vector3(0, 1, 0) },
        new Vertex { Position = new Vector3(0, 1, 1) },
        new Vertex { Position = new Vector3(1, 1, 1) },
        new Vertex { Position = new Vector3(1, 1, 0) },
                                                         
        // Z-                                            
        new Vertex { Position = new Vector3(0, 0, 0) },
        new Vertex { Position = new Vector3(0, 1, 0) },
        new Vertex { Position = new Vector3(1, 1, 0) },
        new Vertex { Position = new Vector3(1, 0, 0) },
                                                         
        // Z+                                            
        new Vertex { Position = new Vector3(1, 0, 1) },
        new Vertex { Position = new Vector3(1, 1, 1) },
        new Vertex { Position = new Vector3(0, 1, 1) },
        new Vertex { Position = new Vector3(0, 0, 1) },
    };
    #endregion
}
