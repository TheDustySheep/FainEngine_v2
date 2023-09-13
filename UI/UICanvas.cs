using FainEngine_v2.Core;
using FainEngine_v2.Entities;
using FainEngine_v2.Rendering.Materials;
using FainEngine_v2.Resources;
using System.Numerics;

namespace FainEngine_v2.UI;
public class UICanvas : IEntity
{
    List<UIElement> Elements = new List<UIElement>();
    UIMesh uiMesh;
    Material material;
    Matrix4x4 model = Matrix4x4.Identity;

    public UICanvas()
    {
        Elements.Add(new UIElement()
        {
            Position = new Vector2(0, 0),
            Size = new Vector2(1, 1),
            Depth = -1,
        });
        uiMesh = new UIMesh();

        material = new Material(ResourceLoader.LoadShader("Resources/GizmoShader"));
    }

    public void Update()
    {
        List<Vertex> vertices = new List<Vertex>();
        List<uint> triangles = new List<uint>();

        uint vertCount = 0;
        foreach (UIElement element in Elements)
        {
            var segment = element.GetMeshSegment();

            for (int i = 0; i < segment.Triangles.Length;  i++)
            {
                segment.Triangles[i] += vertCount;
            }
            
            vertices.AddRange(segment.Vertices);
            triangles.AddRange(segment.Triangles);

            vertCount += (uint)segment.Vertices.Length;
        }

        uiMesh.SetVertices(vertices.ToArray());
        uiMesh.SetTriangles(triangles.ToArray());
        uiMesh.Apply();
        GameGraphics.DrawUIMesh(uiMesh, material, model);
    }
}
