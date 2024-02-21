using FainEngine_v2.Core;
using FainEngine_v2.Entities;
using FainEngine_v2.Rendering.Materials;
using FainEngine_v2.Resources;
using FainEngine_v2.UI.FontSystem;
using FainEngine_v2.UI.UIElements;
using System.Numerics;

namespace FainEngine_v2.UI;
public class UICanvas : IEntity
{
    readonly List<IUIElement> Elements = new List<IUIElement>();
    readonly UIMesh uiMesh;
    readonly Material material;
    Matrix4x4 model = Matrix4x4.Identity;

    public UICanvas()
    {
        FontLoader fontLoader = new FontLoader();
        var font = fontLoader.LoadFont();

        //Elements.Add(new UIColoredBox());
        Elements.Add(new TextElement(font));
        uiMesh = new UIMesh();

        material = new UIMaterial(ResourceLoader.LoadShader("Resources/UI"), font.Texture);
    }

    public void Update()
    {
        List<Vertex> vertices = new List<Vertex>();
        List<uint> triangles = new List<uint>();

        uint vertCount = 0;
        foreach (IUIElement element in Elements)
        {
            var fragment = element.GetMeshSegment(new Rect(-0.5f, -0.5f, 1, 1), 0);
            var segments = fragment.Segments;

            foreach (var segment in segments)
            {
                for (int i = 0; i < segment.Triangles.Length; i++)
                {
                    segment.Triangles[i] += vertCount;
                }

                vertices.AddRange(segment.Vertices);
                triangles.AddRange(segment.Triangles);

                vertCount += (uint)segment.Vertices.Length;
            }
        }

        uiMesh.SetVertices(vertices.ToArray());
        uiMesh.SetTriangles(triangles.ToArray());
        uiMesh.Apply();

        GameGraphics.DrawMesh(uiMesh, material, model);
    }
}
